using AsloobBedaa.DataContext;
using AsloobBedaa.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ExcelDataReader;
using System.Data;

namespace AsloobBedaa.Controllers
{
    public class PayrollMonthlyController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PayrollMonthlyController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: PayrollMonthly
        public async Task<IActionResult> Index()
        {
            var payrolls = await _context.PayrollMonthlies.OrderByDescending(x => x.Month).ToListAsync();
            return View(payrolls);
        }

        // GET: PayrollMonthly/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var payroll = await _context.PayrollMonthlies.FirstOrDefaultAsync(p => p.PayrollId == id);
            if (payroll == null) return NotFound();
            return View(payroll);
        }

        // GET: PayrollMonthly/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: PayrollMonthly/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PayrollMonthly payroll)
        {
            if (ModelState.IsValid)
            {
                payroll.CreatedDate = DateTime.Now;
                payroll.NetPayrollCost = payroll.BasicSalary + payroll.Overtime + payroll.Allowances - payroll.Deductions;
                _context.Add(payroll);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(payroll);
        }

        // GET: PayrollMonthly/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var payroll = await _context.PayrollMonthlies.FindAsync(id);
            if (payroll == null) return NotFound();
            return View(payroll);
        }

        // POST: PayrollMonthly/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PayrollMonthly payroll)
        {
            if (id != payroll.PayrollId) return NotFound();
            if (ModelState.IsValid)
            {
                try
                {
                    payroll.NetPayrollCost = payroll.BasicSalary + payroll.Overtime + payroll.Allowances - payroll.Deductions;
                    _context.Update(payroll);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PayrollMonthlyExists(payroll.PayrollId))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(payroll);
        }

        // GET: PayrollMonthly/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var payroll = await _context.PayrollMonthlies.FirstOrDefaultAsync(p => p.PayrollId == id);
            if (payroll == null) return NotFound();
            return View(payroll);
        }

        // POST: PayrollMonthly/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var payroll = await _context.PayrollMonthlies.FindAsync(id);
            if (payroll != null)
            {
                _context.PayrollMonthlies.Remove(payroll);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: PayrollMonthly/ImportCSV
        public IActionResult ImportCSV()
        {
            var vm = new PayrollCSVModel
            {
                MatchedPayrolls = new List<PayrollMonthly>(),
                NonMatchedPayrolls = new List<PayrollMonthly>()
            };
            return View(vm);
        }

        // POST: PayrollMonthly/UploadCSV
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadCSV(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                TempData["Error"] = "Please select a file to upload.";
                var emptyVm = new PayrollCSVModel
                {
                    MatchedPayrolls = new List<PayrollMonthly>(),
                    NonMatchedPayrolls = new List<PayrollMonthly>()
                };
                return View("ImportCSV", emptyVm);
            }

            try
            {
                var uploadedPayrolls = ReadPayrollsFromFile(file);
                var existingPayrolls = await _context.PayrollMonthlies.ToListAsync();

                var matched = new List<PayrollMonthly>();
                var nonMatched = new List<PayrollMonthly>();

                foreach (var payroll in uploadedPayrolls)
                {
                    if (existingPayrolls.Any(x => x.Month == payroll.Month && x.ProjectName == payroll.ProjectName))
                        matched.Add(payroll);
                    else
                        nonMatched.Add(payroll);
                }

                var vm = new PayrollCSVModel
                {
                    MatchedPayrolls = matched,
                    NonMatchedPayrolls = nonMatched
                };

                TempData["UploadedPayrolls"] = System.Text.Json.JsonSerializer.Serialize(uploadedPayrolls);

                return View("ImportCSV", vm);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error processing file: {ex.Message}";
                var errorVm = new PayrollCSVModel
                {
                    MatchedPayrolls = new List<PayrollMonthly>(),
                    NonMatchedPayrolls = new List<PayrollMonthly>()
                };
                return View("ImportCSV", errorVm);
            }
        }

        // POST: PayrollMonthly/ImportPayrolls
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ImportPayrolls()
        {
            var payrollsJson = TempData["UploadedPayrolls"]?.ToString();
            if (string.IsNullOrEmpty(payrollsJson))
            {
                TempData["Error"] = "No payroll records to import. Please upload a file first.";
                return RedirectToAction("ImportCSV");
            }

            try
            {
                var uploadedPayrolls = System.Text.Json.JsonSerializer.Deserialize<List<PayrollMonthly>>(payrollsJson);
                var existingPayrolls = await _context.PayrollMonthlies.ToListAsync();

                int addedCount = 0, updatedCount = 0, skippedCount = 0;

                foreach (var payroll in uploadedPayrolls)
                {
                    var existing = existingPayrolls.FirstOrDefault(x => x.Month == payroll.Month && x.ProjectName == payroll.ProjectName);
                    if (existing != null)
                    {
                        // Update existing
                        existing.NumberOfEmployees = payroll.NumberOfEmployees;
                        existing.BasicSalary = payroll.BasicSalary;
                        existing.Overtime = payroll.Overtime;
                        existing.Allowances = payroll.Allowances;
                        existing.Deductions = payroll.Deductions;
                        existing.WpsStatus = payroll.WpsStatus ?? existing.WpsStatus;
                        existing.Remarks = payroll.Remarks ?? existing.Remarks;
                        existing.NetPayrollCost = existing.BasicSalary + existing.Overtime + existing.Allowances - existing.Deductions;
                        updatedCount++;
                    }
                    else
                    {
                        // Add new
                        if (string.IsNullOrWhiteSpace(payroll.ProjectName) || payroll.Month == default)
                        {
                            skippedCount++;
                            continue;
                        }
                        payroll.CreatedDate = DateTime.Now;
                        payroll.NetPayrollCost = payroll.BasicSalary + payroll.Overtime + payroll.Allowances - payroll.Deductions;
                        _context.PayrollMonthlies.Add(payroll);
                        addedCount++;
                    }
                }

                await _context.SaveChangesAsync();
                TempData.Remove("UploadedPayrolls");
                TempData["Success"] = $"Import completed! Added: {addedCount}, Updated: {updatedCount}, Skipped: {skippedCount}";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Import failed: {ex.Message}";
            }

            return RedirectToAction("ImportCSV");
        }

        // GET: DownloadTemplate
        public FileResult DownloadTemplate()
        {
            var csv =
                "Month,ProjectName,NumberOfEmployees,BasicSalary,Overtime,Allowances,Deductions,WpsStatus,Remarks\r\n" +
                "2024-01-01,Project Alpha,25,50000.00,5000.00,3000.00,2000.00,Completed,Sample payroll 1\r\n" +
                "2024-02-01,Project Beta,30,60000.00,6000.00,4000.00,2500.00,Pending,Sample payroll 2\r\n";

            var bytes = System.Text.Encoding.UTF8.GetBytes(csv);
            return File(bytes, "text/csv", "payroll-template.csv");
        }

        private bool PayrollMonthlyExists(int id)
        {
            return _context.PayrollMonthlies.Any(e => e.PayrollId == id);
        }

        #region CSV/Excel Import Helper Methods

        private List<PayrollMonthly> ReadPayrollsFromFile(IFormFile file)
        {
            var payrolls = new List<PayrollMonthly>();

            if (file == null || file.Length == 0)
                return payrolls;

            var fileExtension = Path.GetExtension(file.FileName).ToLower();

            if (fileExtension == ".csv")
            {
                payrolls = ReadPayrollsFromCSV(file);
            }
            else if (fileExtension == ".xlsx" || fileExtension == ".xls")
            {
                payrolls = ReadPayrollsFromExcel(file);
            }
            else
            {
                throw new Exception("Unsupported file format. Please upload .csv, .xlsx, or .xls file.");
            }

            return payrolls;
        }

        private List<PayrollMonthly> ReadPayrollsFromCSV(IFormFile file)
        {
            var payrolls = new List<PayrollMonthly>();

            using (var reader = new StreamReader(file.OpenReadStream()))
            {
                var headerLine = reader.ReadLine();
                if (string.IsNullOrWhiteSpace(headerLine))
                    return payrolls;

                var headers = headerLine.Split(',').Select(h => h.Trim()).ToArray();

                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    if (string.IsNullOrWhiteSpace(line))
                        continue;

                    try
                    {
                        var values = line.Split(',');
                        if (values.All(string.IsNullOrWhiteSpace))
                            continue;

                        var payroll = new PayrollMonthly();

                        for (int i = 0; i < headers.Length && i < values.Length; i++)
                        {
                            var header = headers[i];
                            var value = values[i]?.Trim();

                            switch (header)
                            {
                                case "Month":
                                    if (DateTime.TryParse(value, out DateTime month))
                                        payroll.Month = month;
                                    break;
                                case "ProjectName":
                                    payroll.ProjectName = value;
                                    break;
                                case "NumberOfEmployees":
                                    if (int.TryParse(value, out int empCount))
                                        payroll.NumberOfEmployees = empCount;
                                    break;
                                case "BasicSalary":
                                    if (decimal.TryParse(value, out decimal basicSalary))
                                        payroll.BasicSalary = basicSalary;
                                    break;
                                case "Overtime":
                                    if (decimal.TryParse(value, out decimal overtime))
                                        payroll.Overtime = overtime;
                                    break;
                                case "Allowances":
                                    if (decimal.TryParse(value, out decimal allowances))
                                        payroll.Allowances = allowances;
                                    break;
                                case "Deductions":
                                    if (decimal.TryParse(value, out decimal deductions))
                                        payroll.Deductions = deductions;
                                    break;
                                case "WpsStatus":
                                    payroll.WpsStatus = value;
                                    break;
                                case "Remarks":
                                    payroll.Remarks = value;
                                    break;
                            }
                        }

                        payrolls.Add(payroll);
                    }
                    catch
                    {
                        continue;
                    }
                }
            }

            return payrolls;
        }

        private List<PayrollMonthly> ReadPayrollsFromExcel(IFormFile file)
        {
            var payrolls = new List<PayrollMonthly>();
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            using (var stream = file.OpenReadStream())
            using (var reader = ExcelReaderFactory.CreateReader(stream))
            {
                var result = reader.AsDataSet(new ExcelDataSetConfiguration()
                {
                    ConfigureDataTable = (_) => new ExcelDataTableConfiguration() { UseHeaderRow = true }
                });

                var table = result.Tables[0];

                foreach (DataRow row in table.Rows)
                {
                    try
                    {
                        if (row.ItemArray.All(field => field == null || string.IsNullOrWhiteSpace(field.ToString())))
                            continue;

                        DateTime monthValue = DateTime.Now;
                        if (table.Columns.Contains("Month"))
                        {
                            var monthData = row["Month"];
                            if (monthData != null)
                            {
                                if (monthData is DateTime)
                                {
                                    monthValue = (DateTime)monthData;
                                }
                                else if (DateTime.TryParse(monthData.ToString(), out DateTime parsedMonth))
                                {
                                    monthValue = parsedMonth;
                                }
                                else if (double.TryParse(monthData.ToString(), out double serialDate))
                                {
                                    monthValue = DateTime.FromOADate(serialDate);
                                }
                            }
                        }

                        var payroll = new PayrollMonthly
                        {
                            Month = monthValue,
                            ProjectName = table.Columns.Contains("ProjectName") ? row["ProjectName"]?.ToString()?.Trim() : "",
                            NumberOfEmployees = table.Columns.Contains("NumberOfEmployees") && int.TryParse(row["NumberOfEmployees"]?.ToString(), out int empCount) ? empCount : 0,
                            BasicSalary = table.Columns.Contains("BasicSalary") && decimal.TryParse(row["BasicSalary"]?.ToString(), out decimal basicSalary) ? basicSalary : 0,
                            Overtime = table.Columns.Contains("Overtime") && decimal.TryParse(row["Overtime"]?.ToString(), out decimal overtime) ? overtime : 0,
                            Allowances = table.Columns.Contains("Allowances") && decimal.TryParse(row["Allowances"]?.ToString(), out decimal allowances) ? allowances : 0,
                            Deductions = table.Columns.Contains("Deductions") && decimal.TryParse(row["Deductions"]?.ToString(), out decimal deductions) ? deductions : 0,
                            WpsStatus = table.Columns.Contains("WpsStatus") ? row["WpsStatus"]?.ToString()?.Trim() : "",
                            Remarks = table.Columns.Contains("Remarks") ? row["Remarks"]?.ToString()?.Trim() : ""
                        };

                        payrolls.Add(payroll);
                    }
                    catch
                    {
                        continue;
                    }
                }
            }

            return payrolls;
        }

        #endregion
    }
}
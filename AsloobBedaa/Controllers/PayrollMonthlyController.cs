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
            var payrolls = await _context.PayrollMonthlies
                                         .OrderByDescending(x => x.Month)
                                         .ToListAsync();
            return View(payrolls);
        }

        // GET: PayrollMonthly/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: PayrollMonthly/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PayrollMonthly model)
        {
            if (ModelState.IsValid)
            {
                model.CreatedDate = DateTime.Now;

                // Auto-calculate NetPayrollCost
                model.NetPayrollCost = model.BasicSalary + model.Overtime + model.Allowances - model.Deductions;

                _context.PayrollMonthlies.Add(model);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        // GET: PayrollMonthly/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var payroll = await _context.PayrollMonthlies.FindAsync(id);
            if (payroll == null)
                return NotFound();

            return View(payroll);
        }

        // POST: PayrollMonthly/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PayrollMonthly model)
        {
            if (id != model.PayrollId)
                return NotFound();

            if (ModelState.IsValid)
            {
                var existing = await _context.PayrollMonthlies.FindAsync(id);
                if (existing == null)
                    return NotFound();

                existing.Month = model.Month;
                existing.ProjectName = model.ProjectName;
                existing.NumberOfEmployees = model.NumberOfEmployees;
                existing.BasicSalary = model.BasicSalary;
                existing.Overtime = model.Overtime;
                existing.Allowances = model.Allowances;
                existing.Deductions = model.Deductions;
                existing.WpsStatus = model.WpsStatus;
                existing.Remarks = model.Remarks;

                // Recalculate NetPayrollCost
                existing.NetPayrollCost = model.BasicSalary + model.Overtime + model.Allowances - model.Deductions;

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        // GET: PayrollMonthly/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var payroll = await _context.PayrollMonthlies
                                        .FirstOrDefaultAsync(x => x.PayrollId == id);
            if (payroll == null)
                return NotFound();

            return View(payroll);
        }

        // GET: PayrollMonthly/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var payroll = await _context.PayrollMonthlies.FindAsync(id);
            if (payroll == null)
                return NotFound();

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
        public async Task<IActionResult> UploadCSV(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                TempData["Error"] = "Please select a file to upload.";
                return RedirectToAction("ImportCSV");
            }

            var uploadedPayrolls = ReadPayrollsFromFile(file);

            if (uploadedPayrolls.Count == 0)
            {
                TempData["Error"] = "No valid payroll records found in the file.";
                return RedirectToAction("ImportCSV");
            }

            var existingPayrolls = await _context.PayrollMonthlies.ToListAsync();

            var matched = new List<PayrollMonthly>();
            var nonMatched = new List<PayrollMonthly>();

            foreach (var payroll in uploadedPayrolls)
            {
                // Check if payroll exists by Month and ProjectName
                if (existingPayrolls.Any(x => x.Month == payroll.Month && x.ProjectName == payroll.ProjectName))
                    matched.Add(payroll);
                else
                    nonMatched.Add(payroll);
            }

            var viewModel = new PayrollCSVModel
            {
                MatchedPayrolls = matched,
                NonMatchedPayrolls = nonMatched
            };

            // Store in TempData for the import action
            TempData["UploadedPayrolls"] = System.Text.Json.JsonSerializer.Serialize(uploadedPayrolls);

            return View("ImportCSV", viewModel);
        }

        // POST: PayrollMonthly/ImportPayrolls
        [HttpPost]
        public async Task<IActionResult> ImportPayrolls()
        {
            try
            {
                var payrollsJson = TempData["UploadedPayrolls"]?.ToString();
                if (string.IsNullOrEmpty(payrollsJson))
                {
                    TempData["Error"] = "No payroll records to import. Please upload a file first.";
                    return RedirectToAction("ImportCSV");
                }

                var uploadedPayrolls = System.Text.Json.JsonSerializer.Deserialize<List<PayrollMonthly>>(payrollsJson);
                var existingPayrolls = await _context.PayrollMonthlies.ToListAsync();

                int addedCount = 0;
                int updatedCount = 0;
                int skippedCount = 0;

                foreach (var payroll in uploadedPayrolls)
                {
                    // Check if payroll exists by Month and ProjectName
                    var existingPayroll = existingPayrolls.FirstOrDefault(x =>
                        x.Month == payroll.Month && x.ProjectName == payroll.ProjectName);

                    if (existingPayroll != null)
                    {
                        // Update existing payroll
                        existingPayroll.NumberOfEmployees = payroll.NumberOfEmployees;
                        existingPayroll.BasicSalary = payroll.BasicSalary;
                        existingPayroll.Overtime = payroll.Overtime;
                        existingPayroll.Allowances = payroll.Allowances;
                        existingPayroll.Deductions = payroll.Deductions;
                        existingPayroll.WpsStatus = payroll.WpsStatus ?? existingPayroll.WpsStatus;
                        existingPayroll.Remarks = payroll.Remarks ?? existingPayroll.Remarks;

                        // Recalculate NetPayrollCost
                        existingPayroll.NetPayrollCost = existingPayroll.BasicSalary +
                                                        existingPayroll.Overtime +
                                                        existingPayroll.Allowances -
                                                        existingPayroll.Deductions;

                        _context.PayrollMonthlies.Update(existingPayroll);
                        updatedCount++;
                    }
                    else
                    {
                        // Validate required fields
                        if (string.IsNullOrWhiteSpace(payroll.ProjectName) || payroll.Month == default)
                        {
                            skippedCount++;
                            continue;
                        }

                        // Add new payroll
                        payroll.CreatedDate = DateTime.Now;

                        // Calculate NetPayrollCost
                        payroll.NetPayrollCost = payroll.BasicSalary +
                                                payroll.Overtime +
                                                payroll.Allowances -
                                                payroll.Deductions;

                        _context.PayrollMonthlies.Add(payroll);
                        addedCount++;
                    }
                }

                await _context.SaveChangesAsync();

                TempData["Success"] = $"Import completed! Added: {addedCount}, Updated: {updatedCount}, Skipped: {skippedCount}";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error importing payroll records: {ex.Message}";
            }

            return RedirectToAction("ImportCSV");
        }

        // Download CSV Template
        public FileResult DownloadTemplate()
        {
            var csv =
                "Month,ProjectName,NumberOfEmployees,BasicSalary,Overtime,Allowances,Deductions,WpsStatus,Remarks\r\n" +
                "2024-01-01,Project Alpha,25,50000.00,5000.00,3000.00,2000.00,Completed,Sample payroll 1\r\n" +
                "2024-02-01,Project Beta,30,60000.00,6000.00,4000.00,2500.00,Pending,Sample payroll 2\r\n";

            var bytes = System.Text.Encoding.UTF8.GetBytes(csv);

            return File(bytes, "text/csv", "payroll-template.csv");
        }

        // Read payrolls from uploaded file
        private List<PayrollMonthly> ReadPayrollsFromFile(IFormFile file)
        {
            var payrolls = new List<PayrollMonthly>();

            try
            {
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
            }
            catch (Exception)
            {
                throw;
            }

            return payrolls;
        }

        // Read from CSV
        private List<PayrollMonthly> ReadPayrollsFromCSV(IFormFile file)
        {
            var payrolls = new List<PayrollMonthly>();

            using (var reader = new StreamReader(file.OpenReadStream()))
            {
                // Read header line
                var headerLine = reader.ReadLine();
                if (string.IsNullOrWhiteSpace(headerLine))
                    return payrolls;

                var headers = headerLine.Split(',').Select(h => h.Trim()).ToArray();

                // Read data lines
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
                    catch (Exception)
                    {
                        continue;
                    }
                }
            }

            return payrolls;
        }

        // Read from Excel
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

                        // Parse Month with better handling
                        DateTime monthValue = DateTime.Now;
                        if (table.Columns.Contains("Month"))
                        {
                            var monthData = row["Month"];
                            if (monthData != null)
                            {
                                // Try to parse as DateTime directly (Excel date)
                                if (monthData is DateTime)
                                {
                                    monthValue = (DateTime)monthData;
                                }
                                // Try to parse string
                                else if (DateTime.TryParse(monthData.ToString(), out DateTime parsedMonth))
                                {
                                    monthValue = parsedMonth;
                                }
                                // Try to parse as Excel serial date number
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
                    catch (Exception)
                    {
                        continue;
                    }
                }
            }

            return payrolls;
        }
    }
}
namespace AsloobBedaa.Models.Letter
{
    public class LetterTemplateVM
    {
        public int LetterTypeId { get; set; }
        public string Title { get; set; }

        public List<LetterTemplateSection> Sections { get; set; }
    }

}

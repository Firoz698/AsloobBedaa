namespace AsloobBedaa.Models.Letter
{
    public class LetterTemplateSection
    {
        public int Id { get; set; }
        public int LetterTemplateId { get; set; }
        public int SerialNo { get; set; }
        public string Header { get; set; }
        public string Description { get; set; }

        public virtual LetterTemplate LetterTemplate { get; set; }
    }

}

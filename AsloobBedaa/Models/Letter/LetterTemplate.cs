namespace AsloobBedaa.Models.Letter
{
    public class LetterTemplate
    {
        public int Id { get; set; }
        public int LetterTypeId { get; set; }
        public string Title { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }

        public virtual LetterType LetterType { get; set; }
        public virtual ICollection<LetterTemplateSection> Sections { get; set; }
    }

}

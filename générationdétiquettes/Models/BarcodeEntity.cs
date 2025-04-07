namespace générationdétiquettes.Models
{
    public class BarcodeEntity
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public string Base64Image { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }

}

namespace Models
{
    public class Maps
    {
        public int Map_Id { get; set; }
        public int User_Id { get; set; }
        public string Map_Name { get; set; }
        public string Map_Description { get; set; }
        public int? Map_LikeNumber { get; set; }
        public int? Map_NumberCommentary { get; set; }
        public float? Map_TravelTime { get; set; }
        public float? Map_TotalDistance { get; set; }
        public string Map_Image { get; set; }
        public int? Map_Rating { get; set; }
    }
}
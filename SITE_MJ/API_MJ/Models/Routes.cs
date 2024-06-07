namespace Models
{
    public class Routes
    {
        public int Route_Id { get; set; }
        public int Mark_Start { get; set; }
        public int Mark_End { get; set; }
        public string Route_Name { get; set; }
        public string Route_Description { get; set; }
        public float? Route_Distance { get; set; }
    }
}
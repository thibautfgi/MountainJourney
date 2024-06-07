namespace Models
{
   public class Comments
    {
        public int Comment_Id { get; set; }
        public int User_Id { get; set; }
        public int Map_Id { get; set; }
        public string Comment_Content { get; set; }
        public string Comment_Date { get; set; }
    }
}
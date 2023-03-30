namespace CADApp
{
    public class EventArgs
    {
        public class FindClickedArgs
        {
            public string FindText { get; set; }
            public bool IsRegex { get; set; }
            public bool IsCaseInsensitive { get; set; }
        }

        public class ReplaceClickedArgs
        {
            public string FindText { get; set; }
            public string ReplaceText { get; set; }
            public bool IsRegex { get; set; }
            public bool IsCaseInsensitive { get; set; }
        }
    }
}

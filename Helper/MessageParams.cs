

namespace API.Helper
{
    public class MessageParams
    { 
        private const int MaxPageSize = 50;
        public int PageNumber { get; set; } = 1;
        private int _pageSize = 10;

        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
        }


        public string Username { get; set; }
        public string Container { get; set; } = "unread";
    }
}

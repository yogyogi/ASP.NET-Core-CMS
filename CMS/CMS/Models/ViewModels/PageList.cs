using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CMS.Infrastructure;

namespace CMS.Models.ViewModels
{
    public class PageList
    {
        public IEnumerable<Page> page { get; set; }
        public PagingInfo pagingInfo { get; set; }
        public int allTotalPage { get; set; }
        public int activeTotalPage { get; set; }
        public int inactiveTotalPage { get; set; }
        public string searchText { get; set; }
        public int? status { get; set; }
    }
}

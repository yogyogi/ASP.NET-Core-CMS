using CMS.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CMS.Models.ViewModels
{
    public class OrderList
    {
        public IEnumerable<Order> order { get; set; }
        public PagingInfo pagingInfo { get; set; }
        public DateTime? startDate { get; set; }
        public DateTime? endDate { get; set; }
        public string validatedBy { get; set; }
    }
}

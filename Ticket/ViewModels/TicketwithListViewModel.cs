using System.Collections.Generic;
using System.Web.Mvc;
using Type = Ticket.Models.Type;

namespace Ticket.ViewModels
{
    public class TicketwithListViewModel
    {
        public List<SelectListItem> SelectListItems { get; set; }
        public Models.Ticket Ticket { get; set; }

        public TicketwithListViewModel()
        {
            SelectListItems = new List<SelectListItem>();
            SelectListItem Şikayet = new SelectListItem()
            {
                Text = "Şikayet",
                Value = Models.Type.Şikayet.ToString()
            };
            SelectListItem Öneri = new SelectListItem()
            {
                Text = "Öneri",
                Value = Models.Type.Öneri.ToString()
            };
            SelectListItem Görüş = new SelectListItem()
            {
                Text = "Görüş",
                Value = Type.Görüş.ToString()
            };
            SelectListItems.Add(Şikayet);
            SelectListItems.Add(Görüş);
            SelectListItems.Add(Öneri);
        }
    }
}
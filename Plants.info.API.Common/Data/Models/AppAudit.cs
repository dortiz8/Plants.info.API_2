using System;
namespace Plants.info.API.Common.Data.Models
{
	public class AppAudit
	{
        public int Id { get; set; }
        public int ToolId { get; set; }
        public string? Action { get; set; }
        public string? Summary { get; set; }
        public DateTime? DateAdded { get; set; }
    }
}


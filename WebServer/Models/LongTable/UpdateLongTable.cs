using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebServer.Models.LongTable
{
    public class UpdateLongTable
    {
        [Required(ErrorMessage = "长桌ID不为空")]
        public int longTableID { set; get; }

        [Required(ErrorMessage = "长桌参数upNum不为空")]
        public int upNum { set; get; }

        [Required(ErrorMessage = "长桌参数downNum不为空")]
        public int downNum { set; get; }

        [Required(ErrorMessage = "长桌参数leftNum不为空")]
        public int leftNum { set; get; }

        [Required(ErrorMessage = "长桌参数rightNum不为空")]
        public int rightNum { set; get; }
    }
}
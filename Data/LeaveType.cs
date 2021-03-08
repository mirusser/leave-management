﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace leave_management.Data
{
    public class LeaveType
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Range(0, int.MaxValue)]
        public int DeafaultNumberOfDays { get; set; }

        public DateTime DateCreated { get; set; }
    }

    public static class LeaveTypeExtensions
    {
        public static string Fafaf(LeaveType value)
        {
            return "sdfasdf";
        }
    }

}

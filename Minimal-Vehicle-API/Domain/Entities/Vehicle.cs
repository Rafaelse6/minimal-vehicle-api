﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Minimal_Vehicle_API.Domain.Entities
{
    public class Vehicle
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; } = default!;

        [StringLength(150)]
        public string Name { get; set; } = default!;

        [StringLength(100)]
        public string Brand { get; set; } = default!;

        public int Year { get; set; } = default!;
    }
}

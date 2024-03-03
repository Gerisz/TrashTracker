﻿using System.ComponentModel.DataAnnotations.Schema;

namespace CleanTiszaMap.Data.Models.EnumModels
{

    /// <summary>
    /// Sizes of trash, starting from 1
    /// </summary>
    public enum Size
    {
        /// <summary>
        /// Fits in a bag
        /// </summary>
        Bag = 1,

        /// <summary>
        /// Fits in a wheelbarrow
        /// </summary>
        Wheelbarrow,

        /// <summary>
        /// A car is required
        /// </summary>
        Car
    }
}

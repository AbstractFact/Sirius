﻿namespace Sirius.Entities
{
    public class UserSeriesList
    {
        public User User { get; set; }
        public Series Series { get; set; }
        public string Status { get; set; }
        public int Stars { get; set; }
        public string Comment { get; set; }
        public bool Favourite { get; set; }
    }
}

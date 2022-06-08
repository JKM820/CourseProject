//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DatabaseEntities
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Drawing;
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;
    [Serializable]
    public partial class Detail : IEquatable<Detail>
    {
        public Detail()
        {
            Name = "empty";
            Photo = new Bitmap(ConfigurationManager.AppSettings.Get("defaultPhotoPath"));
        }
        public Detail(string name, decimal cost, int vendorCode, Bitmap photo) : base()
        {
            Name = name;
            Cost = cost;
            VendorCode = vendorCode;
            Photo = new Bitmap(ConfigurationManager.AppSettings.Get("defaultPhotoPath"));
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Cost { get; set; }
        public int VendorCode { get; set; }
        public Bitmap Photo { get; set; }
        public byte[] BinaryPhoto
        {
            get
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    BinaryFormatter binaryFormatter = new BinaryFormatter();
                    binaryFormatter.Serialize(ms, Photo);
                    return ms.ToArray();
                }
            }
            set
            {
                using (MemoryStream ms = new MemoryStream(value))
                {
                    BinaryFormatter binaryFormatter = new BinaryFormatter();
                    Photo = binaryFormatter.Deserialize(ms) as Bitmap;
                }
            }
        }

        public bool Equals(Detail other)
        {
            return Name.Equals(other.Name) && Cost.Equals(other.Cost) && VendorCode.Equals(other.VendorCode)
                && Photo.Equals(other.Photo);
        }
    }
}
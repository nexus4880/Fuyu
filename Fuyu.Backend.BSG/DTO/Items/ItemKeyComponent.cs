﻿using System.Runtime.Serialization;

namespace Fuyu.Backend.EFT.DTO.Items
{
    [DataContract]
    public class ItemKeyComponent
    {
        [DataMember]
        public int NumberOfUsages;
    }
}
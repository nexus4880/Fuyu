﻿using System.Runtime.Serialization;

namespace Fuyu.Backend.EFT.DTO.Items
{
    [DataContract]
    public class ItemRecodableComponent
    {
        [DataMember]
        public bool IsEncoded;
    }
}
using System;
using System.Collections.Generic;
using DnDWebAppMVC.Models.Bases;
using Newtonsoft.Json;

namespace DnDWebAppMVC.Models
{
    public class Character
    {
        [JsonProperty(PropertyName = "id")]
        public Guid Id { get; set; }
        [JsonProperty(PropertyName = "ownerId")]
        public Guid OwnerId { get; set; }
        [JsonProperty(PropertyName = "createdOn")]
        public DateTime? CreatedOn { get; set; }
        [JsonProperty(PropertyName = "modifiedOn")]
        public DateTime? ModifiedOn { get; set; }


        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
        [JsonProperty(PropertyName = "race")]
        public string Race { get; set; }
        [JsonProperty(PropertyName = "level")]
        public byte Level { get; set; }
        [JsonProperty(PropertyName = "class")]
        public string Class { get; set; }
        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }


        [JsonProperty(PropertyName = "stats")]
        public List<Quantifiable> Stats { get; set; }
        [JsonProperty(PropertyName = "skills")]
        public List<Quantifiable> Skills { get; set; }
        [JsonProperty(PropertyName = "abilities")]
        public List<Describable> Abilities { get; set; }
        [JsonProperty(PropertyName = "attributes")]
        public List<Describable> Attributes { get; set; }
        [JsonProperty(PropertyName = "items")]
        public List<Describable> Items { get; set; }
        [JsonProperty(PropertyName = "weapons")]
        public List<Describable> Weapons { get; set; }
        [JsonProperty(PropertyName = "spells")]
        public List<Describable> Spells { get; set; }


        [JsonProperty(PropertyName = "copper")]
        public int Copper
        {
            get => Money.GetCopper();
            set => Money.SetMoney(value);
        }
        [JsonIgnore]
        public Money Money { get; set; }

        
        public Character()
        {
            Stats = new List<Quantifiable>();
            Skills = new List<Quantifiable>();
            Abilities = new List<Describable>();
            Attributes = new List<Describable>();
            Items = new List<Describable>();
            Weapons = new List<Describable>();
            Spells = new List<Describable>();

            Money = new Money();
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}

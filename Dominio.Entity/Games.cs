using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Entity
{
    [BsonIgnoreExtraElements]
    public class Games
    {
        public string patient_id { get; set; }
        public string game_name { get; set; }
        public int game_score { get; set; }
        public int game_hits { get; set; }
        public int game_misses { get; set; }
        public DateTime game_played_at { get; set; }
    }



}

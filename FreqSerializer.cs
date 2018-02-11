using Terraria.ModLoader.IO;

namespace PortableStorage
{
    public class FreqSerializer : TagSerializer<Frequency, TagCompound>
    {
        public override TagCompound Serialize(Frequency value) => new TagCompound
        {
            ["left"] = (int)value.colorLeft,
            ["middle"] = (int)value.colorMiddle,
            ["right"] = (int)value.colorRight
        };

        public override Frequency Deserialize(TagCompound tag) => new Frequency((Colors)tag.GetInt("left"), (Colors)tag.GetInt("middle"), (Colors)tag.GetInt("right"));
    }
}
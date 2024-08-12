using System.Runtime.Serialization;

namespace Core.enums
{
    public enum FrameSize
    {
        [EnumMember(Value = "Large")]
        Large,
        [EnumMember(Value = "Medium")]
        Medium,
        [EnumMember(Value = "Small")]
        Small,
    }
}
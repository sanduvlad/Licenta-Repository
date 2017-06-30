using Entities.Attributes;

namespace Entities
{
    public class Coordinates
    {
        [CoodinatesValidator(0, 1, ErrorMessage = "Invalid value")]
        public double U;
        [CoodinatesValidator(0, 1, ErrorMessage = "Invalid value")]
        public double V;

        [CoodinatesValidator(0, 360, ErrorMessage = "Invalid value")]
        public double Rotation;

        [CoodinatesValidator(0, 360, ErrorMessage = "Invalid value")]
        public double Hoz_FOV;
        [CoodinatesValidator(0, 360, ErrorMessage = "Invalid value")]
        public double Vert_FOV;
    }
}
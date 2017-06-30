using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Entities.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    sealed public class CoodinatesValidator : ValidationAttribute
    {
        readonly int _min;
        readonly int _max;

        public CoodinatesValidator(int min, int max)
        {
            this._min = min;
            this._max = max;
        }

        public override bool IsValid(object value)
        {
            return (double)value <= _max && (double)value >= _min;
        }
    }

    public enum CoordType
    {
        U,
        V,
        Rotation,
        Hoz_FOV,
        Vert_FOV
    }
}

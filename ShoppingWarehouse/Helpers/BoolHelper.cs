using ShoppingWarehouse.Resources.Generic;

namespace ShoppingWarehouse.Helpers
{
    public static class BoolHelper
    {
        public static string BoolToYesNoString(bool value)
        {
            return value ? GenericResource.Yes : GenericResource.No;
        }
    }
}

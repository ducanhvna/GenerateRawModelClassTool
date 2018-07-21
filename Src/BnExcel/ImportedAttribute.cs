using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Office.Interop.Excel;

namespace BnExcel
{
    /// <summary>
    /// Excel Data type
    /// </summary>
    public enum ExDataType
    {
        CELL                    = 0,
        ARRAY_VETICAL           = 2,
        ARRAY_HORIZONTAL        = 3,
        LIST_VERTICAL           = 4,
        LIST_HORIZONTAL         = 5
    }
    public class ImportedAttribute : Attribute
    {
        public uint         Size { get; private set; } = 0;
        public ExDataType   DataType { get; private set; } = ExDataType.CELL;
        public string       StartingCell { get; private set; }
        public uint         Count { get; set; } = 0;
        public string       EndSign { get; set; } = "";

        /// <summary>
        /// Constructor with no argument
        /// </summary>
        public ImportedAttribute()
        {
            DataType        = ExDataType.CELL;
            Size     = 1;
            StartingCell    = "A1";
            Count    = 1;
        }

        /// <summary>
        /// Constructor with data in put is single cell
        /// </summary>
        /// <param name="startingCell"></param>
        public ImportedAttribute(string startingCell)
        {
            DataType        = ExDataType.CELL;
            Size            = 1;
            StartingCell    = startingCell;
            Count           = 1;
        }

        /// <summary>
        /// Constructor with data input is array
        /// </summary>
        /// <param name="isVertical"></param>
        /// <param name="startingCell"></param>
        /// <param name="size"></param>
        /// <param name="count"></param>
        public ImportedAttribute(bool isVertical, string startingCell, uint size ,uint count)
        {
            if (startingCell == null)
            {
                throw new ArgumentNullException(nameof(startingCell));
            }

            if (isVertical)
            {
                DataType    = ExDataType.ARRAY_VETICAL;
            }
            else
            {
                DataType    = ExDataType.ARRAY_HORIZONTAL;
            }
            this.StartingCell = startingCell;
            Size            = size;
            Count           = count;
        }

        /// <summary>
        /// constructor with data is List
        /// </summary>
        /// <param name="isVertical"></param>
        /// <param name="startCell"></param>
        /// <param name="size"></param>
        /// <param name="endSign"></param>
        public ImportedAttribute(bool isVertical, string startCell, uint size, string endSign = "")
        {
            if (startCell == null)
            {
                throw new ArgumentNullException(nameof(startCell));
            }

            if (isVertical)
            {
                DataType = ExDataType.LIST_VERTICAL;
            }
            else
            {
                DataType = ExDataType.LIST_HORIZONTAL;
            }

            StartingCell = startCell;
            this.Size = size;
            EndSign = endSign ?? throw new ArgumentNullException(nameof(endSign));
        }
    }
    public static class Importer
    {
        private static Dictionary<string, ExLocation> HeaderCell;
        private static Worksheet _theSheet = null;

        /// <summary>
        /// Initialize data
        /// </summary>
        /// <param name="workSheet"></param>
        /// <param name="EndPoints"></param>
        public static void Initialize(Worksheet workSheet, List<string> EndPoints)
        {
            if (_theSheet != workSheet)
            {
                _theSheet = workSheet;
                HeaderCell = new Dictionary<string, ExLocation>();
                FindAllEndPoint(EndPoints);
            }
        }

        /// <summary>
        /// FindAllEndPoint
        /// </summary>
        /// <param name="endPoints"></param>
        private static void FindAllEndPoint(List<string> endPoints)
        {
            if (endPoints == null)
            {
                return;
            }
            int count = 0;
            for(uint row = 1; row < 150;row++)
            {
                for (uint col = 1; col< 26; col ++)
                {
                    if (count == endPoints.Count)
                    {
                        break;
                    }
                    var value = (string)_theSheet.Cells[row, col].Value;
                    if(value != null)
                    {
                        if(endPoints.Contains(value.Trim()))
                        {
                            if(!HeaderCell.ContainsKey(value))
                            {
                                HeaderCell.Add(value, new ExLocation(row, col));
                                count++;
                                if(count == endPoints.Count)
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Import data from worksheet to 
        /// </summary>
        /// <param name="theSheet"></param>
        /// <param name="obj"></param>
        public static void Import(object obj, uint offSetRow, uint offSetCol)
        {
            if (_theSheet == null)
            {
                throw new ArgumentNullException(nameof(_theSheet));
            }

            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }
            // Get list properties
            var listProperties = obj.GetType().GetProperties().Where(prop => prop.GetCustomAttributes(typeof(ImportedAttribute), false).Any());
            if(listProperties == null)
            {
                throw new ArgumentNullException(nameof(listProperties));
            }
            foreach (var property in listProperties)
            {
                // Get Excel Imported Property Attribute
                var propAttribute = property.GetCustomAttributes(typeof(ImportedAttribute), false).First() as ImportedAttribute;
                if(propAttribute == null)
                {
                    throw new ArgumentNullException(nameof(propAttribute));
                }
                try
                {
                    if(propAttribute.DataType == ExDataType.CELL)
                    {
                        SetValueForSinggleCellSingle(propAttribute, property, obj, offSetRow, offSetCol);
                    }
                    else if(propAttribute.DataType == ExDataType.ARRAY_VETICAL)
                    {
                        SetValueForVerticalArray(propAttribute,property, obj, offSetRow, offSetCol);
                    }
                    else if(propAttribute.DataType == ExDataType.ARRAY_HORIZONTAL)
                    {
                        SetValueForHorizontalArray(propAttribute, property, obj, offSetRow, offSetCol);
                    }
                    else if(propAttribute.DataType == ExDataType.LIST_VERTICAL)
                    {
                        SetValueforVerticalList(propAttribute, property, obj, offSetRow, offSetCol);
                    }
                    else if(propAttribute.DataType == ExDataType.LIST_HORIZONTAL)
                    {
                        SetValueForHorizontalList(propAttribute, property, obj, offSetRow, offSetCol);
                    }
                }
                catch (Exception e)
                {
                    throw e;
                }

            }
        }

        /// <summary>
        /// SetValueForHorizontalList
        /// </summary>
        /// <param name="propAttribute"></param>
        /// <param name="property"></param>
        /// <param name="obj"></param>
        /// <param name="offSetRow"></param>
        /// <param name="offSetCol"></param>
        private static void SetValueForHorizontalList(ImportedAttribute propAttribute, PropertyInfo property, object obj, uint offSetRow, uint offSetCol)
        {
            var propValue = property.GetValue(obj);
            var propType = propValue.GetType();
            if (propType.IsArray)
            {
                var array = propValue as Array;
                var elementType = propType.GetElementType();
                uint offSet = 0;
                var oriCell = FindTheOriginalLocation(propAttribute);
                var newRow = oriCell.Row + offSetRow;

                for (int i = 0; i < array.Length; i++)
                {
                    var newCol = oriCell.Column + offSet + offSetCol;
                    if (propAttribute.EndSign == "")
                    {
                        if (_theSheet.Cells[newRow, newCol].Value == null)
                        {
                            break;
                        }

                    }
                    else
                    {

                        if (HeaderCell.ContainsKey(propAttribute.EndSign))
                        {
                            var endLocation = HeaderCell[propAttribute.EndSign];
                            if (endLocation != null)
                            {
                                if (newRow >= endLocation.Row)
                                {
                                    break;
                                }
                            }
                        }
                    }

                    object value;
                    if (elementType.Assembly.GetName().Name != "mscorlib")
                    {
                        value = Activator.CreateInstance(elementType);
                        array.SetValue(value, i);
                        ImportArrayElement(value, newRow, newCol);
                    }
                    else
                    {
                        if (elementType == typeof(byte))
                        {
                            value = Convert.ToByte(_theSheet.Cells[newRow, newCol].Value as string, 16);
                        }
                        else
                        {
                            value = TryToChangeType(_theSheet.Cells[newRow, newCol].Value, elementType);
                        }
                    }
                    offSet += propAttribute.Size;
                    array.SetValue(value, i);
                }
            }
            else
            {
                throw new Exception("Invalid Import Format of Array!");
            }
        }

        /// <summary>
        /// SetValueforVerticalList
        /// </summary>
        /// <param name="propAttribute"></param>
        /// <param name="property"></param>
        /// <param name="obj"></param>
        /// <param name="offSetRow"></param>
        /// <param name="offSetCol"></param>
        private static void SetValueforVerticalList(ImportedAttribute propAttribute, PropertyInfo property, object obj, uint offSetRow, uint offSetCol)
        {
            var propValue = property.GetValue(obj);
            var propType = propValue.GetType();
            if (propType.IsArray)
            {
                var array = propValue as Array;
                var elementType = propType.GetElementType();
                uint offSet = 0;
                var oriCell = FindTheOriginalLocation(propAttribute);
                var newCol = oriCell.Column + offSetCol;

                for (int i = 0; i < array.Length; i++)
                {
                    var newRow = oriCell.Row + offSet + offSetRow;
                    if (propAttribute.EndSign == "")
                    {
                        if (_theSheet.Cells[newRow, newCol].Value == null)
                        {
                            break;
                        }
                    }
                    else
                    {
                        if (HeaderCell.ContainsKey(propAttribute.EndSign))
                        {
                            var endLocation = HeaderCell[propAttribute.EndSign];
                            if (endLocation != null)
                            {
                                if (newRow >= endLocation.Row)
                                {
                                    break;
                                }
                            }
                        }
                    }

                    object            value;
                    if (elementType.Assembly.GetName().Name != "mscorlib")
                    {
                        value = Activator.CreateInstance(elementType);
                        array.SetValue(value, i);
                        ImportArrayElement(value, newRow, newCol);
                    }
                    else
                    {
                        value = TryToChangeType(_theSheet.Cells[newRow, newCol].Value, elementType);
                    }
                    offSet += propAttribute.Size;
                    array.SetValue(value, i);
                }
            }
            else
            {
                throw new Exception("Invalid Import Format of Array!");
            }

        }

        /// <summary>
        /// FindTheOriginalLocation
        /// </summary>
        /// <param name="propAttribute"></param>
        /// <returns></returns>
        /// <remarks>
        /// Split string by character ','
        /// Find Row in Dictionary
        /// Find Column indictionary
        /// </remarks>
        private static ExLocation FindTheOriginalLocation(ImportedAttribute propAttribute)
        {
            var startingCell = propAttribute.StartingCell;
            // when starting cell contain character ','
            if(startingCell.Contains(","))
            {
                // Split string by character ','
                var splitArray = startingCell.Split(',');
                uint row, column;
                bool isRownummeric = uint.TryParse(splitArray[0].Trim(), out row);
                if(!isRownummeric)
                {
                    // Find Row in Dictionary
                    row = HeaderCell[splitArray[0].Trim()].Row;
                }
                var isColnummeric = uint.TryParse(splitArray[1].Trim(), out column);
                if (!isColnummeric)
                {
                    // Find Column indictionary
                    column = HeaderCell[splitArray[0].Trim()].Column;
                }

                return new ExLocation(row, column);
            }

            // case starting cell normal string 
            var oriCell = _theSheet.Range[propAttribute.StartingCell].Cells[1, 1];
            return new ExLocation((uint)oriCell.Row, (uint)oriCell.Column);
        }

        /// <summary>
        /// SetValueForHorizontalArray
        /// </summary>
        /// <param name="propAttribute"></param>
        /// <param name="property"></param>
        /// <param name="obj"></param>
        private static void SetValueForHorizontalArray(ImportedAttribute propAttribute, PropertyInfo property, object obj, uint offSetRow, uint offSetCol)
        {
            var propValue = property.GetValue(obj);
            var propType = propValue.GetType();
            if (propType.IsArray)
            {
                var array = propValue as Array;
                var elementType = propType.GetElementType();
                uint offSet = 0;
                var oriCell = FindTheOriginalLocation(propAttribute);
                var newRow = oriCell.Row + offSetRow;

                for (int i = 0; i < array.Length; i++)
                {
                    object value = array.GetValue(i);
                    var newCol = oriCell.Column + offSet + offSetCol;
                    if (elementType.Assembly.GetName().Name != "mscorlib")
                    {
                        ImportArrayElement(value, newRow, newCol);
                    }
                    else
                    {
                        if (elementType == typeof(byte))
                        {
                            value = Convert.ToByte(_theSheet.Cells[newRow, newCol].Value as string, 16);
                        }
                        else
                        {
                            value = TryToChangeType(_theSheet.Cells[newRow, newCol].Value, elementType);
                        }
                    }
                    offSet += propAttribute.Size;
                    array.SetValue(value, i);
                }
            }
            else
            {
                throw new Exception("Invalid Import Format of Array!");
            }
        }

        /// <summary>
        /// SetValueForVerticalArray
        /// </summary>
        /// <param name="propAttribute"></param>
        /// <param name="property"></param>
        /// <param name="obj"></param>
        private static void SetValueForVerticalArray(ImportedAttribute propAttribute, PropertyInfo property, object obj, uint offSetRow, uint offSetCol)
        {
            var propValue = property.GetValue(obj);
            var propType = propValue.GetType();
            if (propType.IsArray)
            {
                var array = propValue as Array;
                var elementType = propType.GetElementType();
                uint offSet = 0;
                var oriCell = FindTheOriginalLocation(propAttribute);
                var newCol = oriCell.Column + offSetCol;

                for (int i = 0; i < array.Length; i++)
                {
                    var newRow = oriCell.Row + offSet + offSetRow;
                   
                    object value = array.GetValue(i);
                    if (elementType.Assembly.GetName().Name != "mscorlib")
                    {
                        ImportArrayElement(value, newRow, newCol);
                    }
                    else
                    {
                        value = TryToChangeType(_theSheet.Cells[newRow, newCol].Value, elementType);
                    }
                    offSet += propAttribute.Size;
                    array.SetValue(value, i);
                }
            }
            else
            {
                throw new Exception("Invalid Import Format of Array!");
            }
        }

        /// <summary>
        /// ImportArrayElement
        /// </summary>
        /// <param name="obj"></param>
        private static void ImportArrayElement(object obj, uint offSetRow, uint offSetCol)
        {
            var listProperties = obj.GetType().GetProperties().Where(prop => prop.GetCustomAttributes(typeof(ImportedAttribute), false).Any());
            foreach (var property in listProperties)
            {
                // Get Excel Imported Property Attribute
                var propAttribute = property.GetCustomAttributes(typeof(ImportedAttribute), false).First() as ImportedAttribute;
                if (propAttribute == null)
                {
                    throw new ArgumentNullException(nameof(propAttribute));
                }
                try
                {
                    if (propAttribute.DataType == ExDataType.CELL)
                    {
                        SetValueForSinggleCellSingle(propAttribute, property, obj, offSetRow, offSetCol);
                    }
                    else if (propAttribute.DataType == ExDataType.ARRAY_VETICAL)
                    {
                        SetValueForVerticalArray(propAttribute, property, obj, offSetRow, offSetCol);
                    }
                    else if (propAttribute.DataType == ExDataType.ARRAY_HORIZONTAL)
                    {
                        SetValueForHorizontalArray(propAttribute, property, obj, offSetRow, offSetCol);
                    }
                    else if (propAttribute.DataType == ExDataType.LIST_VERTICAL)
                    {
                        SetValueforVerticalList(propAttribute, property, obj, offSetRow, offSetCol);
                    }
                    else if (propAttribute.DataType == ExDataType.LIST_HORIZONTAL)
                    {
                        SetValueForHorizontalList(propAttribute, property, obj, offSetRow, offSetCol);
                    }
                }
                catch(Exception e)
                {
                    throw e;
                }
            }
        }

        /// <summary>
        /// SetValueForSinggleCellSingle
        /// </summary>
        /// <param name="propAttribute"></param>
        /// <param name="property"></param>
        /// <param name="obj"></param>
        private static void SetValueForSinggleCellSingle(ImportedAttribute propAttribute, PropertyInfo property, object obj, uint offSetRow, uint offSetCol)
        {
            var propValue = property.GetValue(obj);
            if(propValue == null)
            {
                throw new NullReferenceException(nameof(obj));
            }
            var propType = propValue.GetType();
            var oriCell = FindTheOriginalLocation(propAttribute);
            var newCol = oriCell.Column + offSetCol;

            if (propType.IsArray == false)                       // Import Others
            {
                var newRow = oriCell.Row + offSetRow;
                if (property.PropertyType.Assembly.GetName().Name != "mscorlib")                         // Import User-defined Types
                {
                    Import(propValue, newRow, newCol);
                }
                else                                                                                    // Import Basic Types
                {
                    propValue = TryToChangeType(_theSheet.Cells[newRow, newCol].Value, propType);
                }
                // Set imported value
                property.SetValue(obj, propValue);
            }
            else                                                                                        // Invalid Format
            {
                throw new Exception("Type of Attribute is invalid: SetValueForSinggleCellSingle");
            }
        }

        /// <summary>
        /// Conver from Hexa to int
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static int FromHex(string value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }
            // strip the leading 0x
            if (value.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
            {
                value = value.Substring(2);
            }
            return Int32.Parse(value, NumberStyles.HexNumber);
        }

        /// <summary>
        /// TryToChangeType
        /// </summary>
        /// <param name="value"></param>
        /// <param name="conversionType"></param>
        /// <returns></returns>
        private static object TryToChangeType(object value, Type conversionType)
        {
            if (value == null)
            {
                if(conversionType == typeof (string))
                {
                    return "";
                }
                throw new ArgumentNullException(nameof(value));
            }

            if (conversionType == null)
            {
                throw new ArgumentNullException(nameof(conversionType));
            }

            if (value.GetType() == typeof(string) && conversionType == typeof(Int32))
            {
                return FromHex((string)value);
            }
            return Convert.ChangeType(value, conversionType);
        }
    }
}

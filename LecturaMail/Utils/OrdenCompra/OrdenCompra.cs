using System.Collections.Generic;
using System.Linq;

namespace LecturaMail.Utils.OrdenCompra
{
    public class OrdenCompra
    {
        public OrdenCompra()
        {
            Items = new List<Item>();
            Rut = "";
            NumeroCompra = "";
            CentroCosto = "";
            Observaciones = "";
            Direccion = "";
            TipoPareoCentroCosto = TipoPareoCentroCosto.SinPareo;
        }
        
        public TipoPareoCentroCosto TipoPareoCentroCosto { get; set; }

        public string Direccion
        {
            get { return _direccion.Replace("'", "''"); }
            set { _direccion = value; }
        }

        private string _direccion;
        
        public string Rut
        {
            get { return _rut; }
            set
            {
                _rut = value.Replace(" ","");
                if (_rut.Contains('-'))
                {
                    _rut = _rut.Split('-')[0];
                }
                _rut = _rut.Replace(".", "");
            }
        }

        private string _rut;
        public string NumeroCompra { get; set; }
        public string CentroCosto { get; set; }
        public string Observaciones { get; set; }
        public List<Item> Items { get; set; }

        public void AddItem(Item it)
        {
            Items.Add(it);
        }

        public override string ToString()
        {
            //var items = Items.Aggregate("", (current, item) => current + string.Format($"{item}\n"));
            var cont = 1;
            var items = Items.Aggregate("", (current, ir) => current + $"{cont++}.- {ir}\n");
            return $"Rut: {Rut}, N° Compra: {NumeroCompra}, Centro Costo: {CentroCosto}, Observaciones: {Observaciones}, Dirección: {Direccion}\nItems:\n{items}";
        }

    }

    /// <summary>
    /// Tipo de Pareo de Centro de Costo
    /// </summary>
    public enum TipoPareoCentroCosto
    {
        SinPareo = 0,
        PareoDescripcionExacta = 1,
        PareoDescripcionLike = 2,
        PareoDescripcionMatch = 3
    }

    public class OrdenCompraSecuritas : OrdenCompra
    {

        public OrdenCompraSecuritas()
        {
            ItemsSecuritas = new List<ItemSecuritas>();
        }

        public List<ItemSecuritas> ItemsSecuritas { get; set; }

        public void AddItemSecuritas(ItemSecuritas it)
        {
            ItemsSecuritas.Add(it);
        }

        public override string ToString()
        {
            var items = ItemsSecuritas.Aggregate("", (current, item) => current + string.Format($"{item}\n"));
            return $"Rut: {Rut}, N° Compra: {NumeroCompra}, Centro Costo: {CentroCosto}, Observaciones: {Observaciones}, Dirección: {Direccion}\nItems:\n{items}";
        }
    }

    public class OrdenCompraCarozzi : OrdenCompra
    {
        public OrdenCompraCarozzi()
        {
            ItemsCarozzi = new List<ItemCarozzi>();
        }

        public List<ItemCarozzi> ItemsCarozzi { get; set; }

        public void AddItemCarozzi(ItemCarozzi it)
        {
            ItemsCarozzi.Add(it);
        }

        public override string ToString()
        {
            var items = ItemsCarozzi.Aggregate("", (current, item) => current + string.Format($"{item}\n"));
            return $"Rut: {Rut}, N° Compra: {NumeroCompra}, Centro Costo: {CentroCosto}, Observaciones: {Observaciones}, Dirección: {Direccion}\nItems:\n{items}";
        }
    }

    public class OrdenCompraClinicaDavila : OrdenCompra
    {
        public OrdenCompraClinicaDavila()
        {
            ItemsClinicaDavila = new List<ItemDavila>();
        }
        public List<ItemDavila> ItemsClinicaDavila { get; set; }

        public void AddItemDavila(ItemDavila it)
        {
            ItemsClinicaDavila.Add(it);
        }
        public override string ToString()
        {
            var items = ItemsClinicaDavila.Aggregate("", (current, item) => current + string.Format($"{item}\n"));
            return $"Rut: {Rut}, N° Compra: {NumeroCompra}, Centro Costo: {CentroCosto}, Observaciones: {Observaciones}, Dirección: {Direccion}\nItems:\n{items}";
        }
    }
}
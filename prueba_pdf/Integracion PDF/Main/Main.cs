using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Hardcodet.Wpf.TaskbarNotification;
using IntegracionPDF.Integracion_PDF.Utils;
using IntegracionPDF.Integracion_PDF.Utils.Email;
using IntegracionPDF.Integracion_PDF.Utils.Integracion.PDF.ACH;
using IntegracionPDF.Integracion_PDF.Utils.Integracion.PDF.AIEP;
using IntegracionPDF.Integracion_PDF.Utils.Integracion.PDF.Alsacia;
using IntegracionPDF.Integracion_PDF.Utils.Integracion.PDF.Arauco;
using IntegracionPDF.Integracion_PDF.Utils.Integracion.PDF.BHPBilliton;
using IntegracionPDF.Integracion_PDF.Utils.Integracion.PDF.Candem;
using IntegracionPDF.Integracion_PDF.Utils.Integracion.PDF.Carozzi;
using IntegracionPDF.Integracion_PDF.Utils.Integracion.PDF.Cencosud;
using IntegracionPDF.Integracion_PDF.Utils.Integracion.PDF.Clariant;
using IntegracionPDF.Integracion_PDF.Utils.Integracion.PDF.ClinicaAlemana;
using IntegracionPDF.Integracion_PDF.Utils.Integracion.PDF.ClinicaDavila;
using IntegracionPDF.Integracion_PDF.Utils.Integracion.PDF.ClinicaLilas;
using IntegracionPDF.Integracion_PDF.Utils.Integracion.PDF.ClinicaUniversidadAndes;
using IntegracionPDF.Integracion_PDF.Utils.Integracion.PDF.ConsorcioCompaniaSeguridad;
using IntegracionPDF.Integracion_PDF.Utils.Integracion.PDF.ConstructoraIngevec;
using IntegracionPDF.Integracion_PDF.Utils.Integracion.PDF.Dellanatura;
using IntegracionPDF.Integracion_PDF.Utils.Integracion.PDF.Dole;
using IntegracionPDF.Integracion_PDF.Utils.Integracion.PDF.Ecogestion;
using IntegracionPDF.Integracion_PDF.Utils.Integracion.PDF.ExpressSantiago;
using IntegracionPDF.Integracion_PDF.Utils.Integracion.PDF.FoodFantasy;
using IntegracionPDF.Integracion_PDF.Utils.Integracion.PDF.GestionPersonasServiciosLtda;
using IntegracionPDF.Integracion_PDF.Utils.Integracion.PDF.HormigonesTransex;
using IntegracionPDF.Integracion_PDF.Utils.Integracion.PDF.Iansa;
using IntegracionPDF.Integracion_PDF.Utils.Integracion.PDF.Indra;
using IntegracionPDF.Integracion_PDF.Utils.Integracion.PDF.IntegraMedica;
using IntegracionPDF.Integracion_PDF.Utils.Integracion.PDF.IsapreConsalud;
using IntegracionPDF.Integracion_PDF.Utils.Integracion.PDF.Komatsu;
using IntegracionPDF.Integracion_PDF.Utils.Integracion.PDF.Megasalud;
using IntegracionPDF.Integracion_PDF.Utils.Integracion.PDF.MTS;
using IntegracionPDF.Integracion_PDF.Utils.Integracion.PDF.OfficeStore;
using IntegracionPDF.Integracion_PDF.Utils.Integracion.PDF.PizzaHut;
using IntegracionPDF.Integracion_PDF.Utils.Integracion.PDF.Securitas;
using IntegracionPDF.Integracion_PDF.Utils.Integracion.PDF.ServiciosAndinos;
using IntegracionPDF.Integracion_PDF.Utils.Integracion.PDF.TNT;
using IntegracionPDF.Integracion_PDF.Utils.Integracion.PDF.Toc;
using IntegracionPDF.Integracion_PDF.Utils.Integracion.PDF.UDLA;
using IntegracionPDF.Integracion_PDF.Utils.Integracion.PDF.UNAB;
using IntegracionPDF.Integracion_PDF.Utils.Integracion.PDF.VitaminaWorkLife;
using IntegracionPDF.Integracion_PDF.Utils.Integracion.XLSX.Flores;
using IntegracionPDF.Integracion_PDF.Utils.Oracle.DataAccess;
using IntegracionPDF.Integracion_PDF.Utils.OrdenCompra;
using IntegracionPDF.Integracion_PDF.Utils.OrdenCompra.Integracion;
using IntegracionPDF.Integracion_PDF.Utils.OrdenCompra.Integracion.OrdenCompraDataAdapter;
using IntegracionPDF.Integracion_PDF.View;
using IntegracionPDF.Integracion_PDF.ViewModel;
using IntegracionPDF.Integracion_PDF.Utils.Integracion.PDF.KaeferBuildtek;
using IntegracionPDF.Integracion_PDF.Utils.Integracion.PDF.AsesoriasServiciosCapacitacionIcyde;
using IntegracionPDF.Integracion_PDF.Utils.Integracion.PDF.Intertek;
using IntegracionPDF.Integracion_PDF.Utils.Integracion.PDF.AguasAndinas;
using IntegracionPDF.Integracion_PDF.Utils.Integracion.PDF.AFPHabitat;
using IntegracionPDF.Integracion_PDF.Utils.Integracion.PDF.CorporacionDesarrolloTecnologico;
using IntegracionPDF.Integracion_PDF.Utils.Integracion.PDF.Ingeproject;
using IntegracionPDF.Integracion_PDF.Utils.Integracion.PDF.SociedadEducacionalAraucana;
using IntegracionPDF.Integracion_PDF.Utils.Integracion.PDF.DunkinDonuts;
using IntegracionPDF.Integracion_PDF.Utils.Integracion.PDF.Teveuk;
using IntegracionPDF.Integracion_PDF.Utils.Integracion.PDF.ColegioCoya;
using IntegracionPDF.Integracion_PDF.Utils.Integracion.PDF.SociedadInstruccion;
using IntegracionPDF.Integracion_PDF.Utils.Integracion.PDF.ShawAlmexChile;
using IntegracionPDF.Integracion_PDF.Utils.Integracion.PDF.UnitedNations;
using IntegracionPDF.Integracion_PDF.Utils.Integracion.PDF.IngenieriaComercializadoraRiego;
using IntegracionPDF.Integracion_PDF.Utils.Integracion.PDF.EnvasadosMovipackChile;
using IntegracionPDF.Integracion_PDF.Utils.Integracion.PDF.LarrainSalas;
using IntegracionPDF.Integracion_PDF.Utils.Integracion.PDF.LaboratorioLBC;
using IntegracionPDF.Integracion_PDF.Utils.Integracion.PDF.ProyektaSA;
using IntegracionPDF.Integracion_PDF.Utils.Integracion.PDF.BiomedicalDistributionChile;
using IntegracionPDF.Integracion_PDF.Utils.Integracion.PDF.AridosSantaFeSA;
using IntegracionPDF.Integracion_PDF.Utils.Integracion.PDF.CementosTransex;
using IntegracionPDF.Integracion_PDF.Utils.Integracion.PDF.CerveceriaCCU;
using IntegracionPDF.Integracion_PDF.Utils.Integracion.PDF.DepositosContenedores;
using IntegracionPDF.Integracion_PDF.Utils.Integracion.PDF.ConstructoraStaFe;
using IntegracionPDF.Integracion_PDF.Utils.Integracion.PDF.Ezentis;
using IntegracionPDF.Integracion_PDF.Utils.Integracion.PDF.SeidorChile;
using IntegracionPDF.Integracion_PDF.Utils.Integracion.PDF.cftSanAgustin;
using IntegracionPDF.Integracion_PDF.Utils.Integracion.PDF.Rimasa;
using IntegracionPDF.Integracion_PDF.Utils.Integracion.PDF.Eulen;
using IntegracionPDF.Integracion_PDF.Utils.Integracion.PDF.Petrobras;
using IntegracionPDF.Integracion_PDF.Utils.Integracion.PDF.AlimentosSanMartin;
using IntegracionPDF.Integracion_PDF.Utils.Integracion.PDF.Zical;
using IntegracionPDF.Integracion_PDF.Utils.Integracion.PDF.ClubAereoCarabineros;
using IntegracionPDF.Integracion_PDF.Utils.Integracion.PDF.Report;
using IntegracionPDF.Integracion_PDF.Utils.Integracion.PDF.TecnologiaTrasnporteMinerales;

namespace IntegracionPDF.Integracion_PDF.Main
{
    public static class Main
    {
        #region Variables

        public static bool OnlyOne;

        #endregion  

        #region Delete/Move File
        public static void MoveFileToProcessFolder(string pdfPath, OrdenCompra o)
        {
            if (OnlyOne) return;
            if (InternalVariables.IsDebug()) return;
            var tmpFileName = pdfPath.Substring(pdfPath.LastIndexOf(@"\", StringComparison.Ordinal) + 1); ;
            try
            {
                var razon = OracleDataAccess.GetRazonSocial(o.Rut);
                var folderName = $"{o.Rut}-{razon.FormattFolderName()}";
                var folderRooth = $@"{InternalVariables.GetOcProcesadasFolder()}{folderName}\";
                var fileName = $"{DateTime.Now:dd-MM-yyyy-HH-mm-ss}_{tmpFileName}";
                //var fileName = $"{tmpFileName}";
                if (!Directory.Exists(folderRooth))
                    Directory.CreateDirectory(folderRooth);
                Console.WriteLine($"Move: {pdfPath} \n to: {folderRooth}{fileName}");
                File.Move(pdfPath, $"{folderRooth}{fileName}");
            }
            catch (Exception e)
            {
                Log.TryError($"Error al momento de mover el archivo: {tmpFileName}, a carpetas de Ordenes Procesadas..");
                Log.TryError(e.Message);
                Log.TryError(e.ToString());
            }
        }

        public static void DeleteFile(string pdfPath)
        {
            //if (_onlyOne) return;
            //if (InternalVariables.IsDebug()) return;
            var fileName = pdfPath.Substring(pdfPath.LastIndexOf(@"\", StringComparison.Ordinal) + 1);
            try
            {
                Console.WriteLine($"Delete: {pdfPath}");
                File.Delete(pdfPath);
            }
            catch (Exception e)
            {
                Log.TryError($"Error al momento de Eliminar el archivo: {fileName}...");
                Log.TryError(e.Message);
                Log.TryError(e.ToString());
            }
        }


        #endregion

        private static void ExecuteSinglePdf(PDFReader pdfReader)
        {
            var option = GetOptionNumber(pdfReader);
            OrdenCompra ordenCompra = null;
            var ocAdapter = new OrdenCompraIntegracion();
            var ocAdapterList = new List<OrdenCompraIntegracion>();
            Console.WriteLine($"FIRST: {option}");
            switch (option)
            {
                case 0:
                    var easy = new Easy(pdfReader);
                    ordenCompra = easy.GetOrdenCompra();
                    ocAdapter = ordenCompra.AdapterCencosudFormatToCompraIntegracion();
                    break;
                case 1:
                    var cencosudRetailSa1 = new CencosudRetailSa(pdfReader);
                    ordenCompra = cencosudRetailSa1.GetOrdenCompra();
                    ocAdapter = ordenCompra.AdapterCencosudFormatToCompraIntegracion();
                    break;
                case 2:
                    var cencosudRetailSa = new CencosudRetailSa(pdfReader);
                    ordenCompra = cencosudRetailSa.GetOrdenCompra();
                    ocAdapter = ordenCompra.AdapterCencosudFormatToCompraIntegracion();
                    break;
                case 3:
                    var indra = new Indra(pdfReader);
                    ordenCompra = indra.GetOrdenCompra();

                    break;
                case 5:
                    var unab = new Unab(pdfReader);
                    ordenCompra = unab.GetOrdenCompra();
                    ocAdapter = ordenCompra.AdapterUnabFormatToCompraIntegracion();
                    break;
                case -5:
                    var corpUnab = new CorpUnab(pdfReader);
                    ordenCompra = corpUnab.GetOrdenCompra();
                    ocAdapter = ordenCompra.AdapterUnabFormatToCompraIntegracion();
                    break;
                case 6:
                    var esach = new Esach(pdfReader);
                    ordenCompra = esach.GetOrdenCompra();
                    ocAdapter = ordenCompra.AdapterEsachFormatToCompraIntegracion();
                    break;
                case 7:
                    var bhpBilliton = new BhpBilliton(pdfReader);
                    ordenCompra = bhpBilliton.GetOrdenCompra();
                    ocAdapter = ordenCompra.AdapterBhpBillitonFormatToCompraIntegracion();
                    break;

                case 8:
                    var clinicaDavila = new ClinicaDavila(pdfReader);
                    if (clinicaDavila.HaveAnexo())
                    {
                        var ordenes = clinicaDavila.GetOrderFromAnexo();
                        ocAdapterList = ordenes.Select(or => or.AdapterClinicaDavilaFormatToCompraIntegracionWithMatchCencos()).ToList();
                        option = -8;
                    }
                    else
                    {
                        var ordenCompraD = clinicaDavila.GetOrdenCompraProcesada();
                        //ordenCompraD.CentroCosto = "0";
                        ocAdapter = ordenCompraD.AdapterClinicaDavilaFormatToCompraIntegracionWithMatchCencos();
                    }
                    break;
                case 9:
                    var carozzi = new Carozzi(pdfReader);
                    var ordenCompraCarozzi = carozzi.GetOrdenCompra();
                    ordenCompra = ordenCompraCarozzi;//carozzi.GetOrdenCompra();
                    ocAdapter = ordenCompraCarozzi.AdapterCarozziFormatToCompraIntegracion();
                    break;
                case 10:
                    var securitasAustral = new SecuritasAustral(pdfReader);
                    var ordenCompraSa = securitasAustral.GetOrdenCompra();
                    ordenCompra = ordenCompraSa;
                    ocAdapterList = ordenCompraSa.AdapterSecuritasAustralFormatToCompraIntegracion();
                    break;
                case 4:
                    var securitas = new Securitas(pdfReader);
                    var ordeCompraS = securitas.GetOrdenCompra();
                    ordenCompra = ordeCompraS;
                    ocAdapterList = ordeCompraS.AdapterSecuritasAustralFormatToCompraIntegracionWithBodega();
                    break;
                case 12:
                    var securitasCapacitaciones = new SecuritasAustral(pdfReader);
                    var ordenCompraCa = securitasCapacitaciones.GetOrdenCompra();
                    ordenCompra = ordenCompraCa;
                    ocAdapterList = ordenCompraCa.AdapterSecuritasAustralFormatToCompraIntegracion();
                    break;
                case 11:
                    var dole = new Dole(pdfReader);
                    ordenCompra = dole.GetOrdenCompra();
                    //ocAdapter = ordenCompra.AdapterDoleFormatToCompraIntegracion();
                    break;
                case 13:
                    var udla = new Udla(pdfReader);
                    ordenCompra = udla.GetOrdenCompra();
                    ocAdapter = ordenCompra.AdapterUnabFormatToCompraIntegracion();
                    break;
                case 14:
                    var aiep = new Aiep(pdfReader);
                    ordenCompra = aiep.GetOrdenCompra();
                    ocAdapter = ordenCompra.AdapterUnabFormatToCompraIntegracion();
                    break;
                case 15:
                    var cliniAlemana = new ClinicaAlemana(pdfReader);
                    ordenCompra = cliniAlemana.GetOrdenCompra();
                    ocAdapter = ordenCompra.AdapterClinicaAlemanaFormatToCompraIntegracion();
                    break;
                case -15:
                    var clinicaAlemanaArtikosFormat = new ClinicaAlemanaArtikosFormat(pdfReader);
                    ordenCompra = clinicaAlemanaArtikosFormat.GetOrdenCompra();
                    ocAdapter = ordenCompra.AdapterClinicaAlemanaFormatToCompraIntegracion();
                    break;
                case 16:
                    var uvm = new Udla(pdfReader);
                    ordenCompra = uvm.GetOrdenCompra();
                    ocAdapter = ordenCompra.AdapterUnabFormatToCompraIntegracion();
                    break;
                case 17:
                    var dellanatura = new Dellanatura(pdfReader);
                    ordenCompra = dellanatura.GetOrdenCompra();
                    ocAdapter = ordenCompra.AdapterGenericFormatWithSkuToCompraIntegracion();
                    break;
                case 18:
                    var toc = new Toc(pdfReader);
                    ordenCompra = toc.GetOrdenCompra();
                    ocAdapter = ordenCompra.AdapterGenericFormatWithSkuToCompraIntegracion();
                    break;
                case 19:
                    var komatsu = new Komatsu(pdfReader);
                    ordenCompra = komatsu.GetOrdenCompra();
                    ocAdapter = ordenCompra.AdapterGenericFormatWithSkuToCompraIntegracion();
                    foreach (var a in ocAdapter.DetallesCompra)
                    {
                        a.CodigoBodega = 66;
                    }
                    break;
                case 20:
                    var komatsuCummins = new KomatsuCummins(pdfReader);
                    ordenCompra = komatsuCummins.GetOrdenCompra();
                    ocAdapter = ordenCompra.AdapterGenericFormatWithSkuToCompraIntegracion();
                    foreach (var a in ocAdapter.DetallesCompra)
                    {
                        a.CodigoBodega = 66;
                    }
                    break;
                case 21:
                    var alsacia = new Alsacia(pdfReader);
                    alsacia.GetOrdenCompra2();//ordenCompra =
                    //ocAdapter = ordenCompra.AdapterGenericFormatWithSkuToCompraIntegracion();
                    ocAdapterList.AddRange(alsacia.GetOrdenCompra2().Select(ord => ord.AdapterGenericFormatWithSkuAndNumericCencosToCompraIntegracion()));
                    break;
                case 22:
                    var expressSantiago = new ExpressSantiago(pdfReader);
                    expressSantiago.GetOrdenCompra2();//ordenCompra =
                    //ocAdapter = ordenCompra.AdapterGenericFormatWithSkuToCompraIntegracion();
                    ocAdapterList.AddRange(expressSantiago.GetOrdenCompra2().Select(ord => ord.AdapterGenericFormatWithSkuAndNumericCencosToCompraIntegracion()));
                    break;
                case 23:
                    var isapreConsalud = new IsapreConsalud(pdfReader);
                    ordenCompra = isapreConsalud.GetOrdenCompra();
                    ocAdapter = ordenCompra.AdapterGenericFormatWithSkuAndNumericCencosToCompraIntegracion();
                    break;
                case 24:
                    var iansa = new Iansa(pdfReader);
                    ordenCompra = iansa.GetOrdenCompra();
                    ocAdapter = ordenCompra.AdapterIansaFormatToCompraIntegracion();
                    break;
                case 25:
                    var tnt = new TNT(pdfReader);
                    ordenCompra = tnt.GetOrdenCompra();
                    //foreach(var o in ordenCompra.Items)
                    //{
                    //    if (o.Sku.Equals("P107404"))
                    //    {
                    //        o.Cantidad = "1";
                    //    }
                    //}
                    ocAdapter = ordenCompra.AdapterGenericFormatWithSkuAndDescriptionCencosWithMatchToCompraIntegracion();
                    break;
                case 26:
                    var pizzaHut = new PizzaHut(pdfReader);
                    ordenCompra = pizzaHut.GetOrdenCompra();
                    ocAdapter = ordenCompra.AdapterGenericFormatToCompraIntegracion();
                    break;
                case 27:
                    var constructoraIngevec = new ConstructoraIngevec(pdfReader);
                    ordenCompra = constructoraIngevec.GetOrdenCompra();
                    ocAdapter = ordenCompra.AdapterGenericFormatWithSkuToCompraWithStockW102030Integracion();
                    break;
                case 28:
                    var vitaminaWorkLife = new VitaminaWorkLife(pdfReader);
                    ordenCompra = vitaminaWorkLife.GetOrdenCompra();
                    ocAdapter = ordenCompra.AdapterGenericFormatWithSkuToCompraIntegracion();
                    break;
                case 29:
                    var mts = new Mts(pdfReader);
                    ordenCompra = mts.GetOrdenCompra();
                    ocAdapter =
                        ordenCompra.AdapterGenericFormatWithSkuAndDescriptionCencosWithMatchToCompraIntegracion();
                    break;
                case 30:
                    var clariant = new Clariant(pdfReader);
                    ordenCompra = clariant.GetOrdenCompra();
                    ocAdapter = ordenCompra.AdapterGenericFormatWithSkuAndDescriptionCencosWithMatchToCompraIntegracion();
                    break;
                case 31:
                    var candem = new Candem(pdfReader);
                    ocAdapterList.AddRange(candem.GetOrdenCompra2().Select(ord => ord.AdapterGenericFormatWithSkuToCompraIntegracion()));
                    break;
                case 32:
                    var serviciosAndinos = new ServicioAndinos(pdfReader);
                    ordenCompra = serviciosAndinos.GetOrdenCompra();
                    ocAdapter = ordenCompra.AdapterUnabFormatToCompraIntegracion();
                    break;
                case 33:
                    var gestionPersona = new GestionPersonasServiciosLtda(pdfReader);
                    ordenCompra = gestionPersona.GetOrdenCompra();
                    ocAdapter = ordenCompra.AdapterGenericFormatWithSkuAndNumericCencosToCompraIntegracion();
                    break;
                case 34:
                    var hormigonesTransex = new HormigonesTransex(pdfReader);
                    ordenCompra = hormigonesTransex.GetOrdenCompra();
                    break;
                case 35:
                    var officeStore = new OfficeStore(pdfReader);
                    ordenCompra = officeStore.GetOrdenCompra();
                    ocAdapter = ordenCompra.AdapterGenericFormatWithSkuToCompraIntegracion();
                    break;
                case 36:
                    var clinicaLila = new ClinicaLilas(pdfReader);
                    ordenCompra = clinicaLila.GetOrdenCompra();
                    ocAdapter = ordenCompra.AdapterGenericFormatWithDescriptionCencosToCompraIntegracion();
                    break;
                case 37:
                    //TODO ABENGOA
                    break;
                case 38:
                    var clinicaAndes = new ClinicaUniversidadAndes(pdfReader);
                    ordenCompra = clinicaAndes.GetOrdenCompra();
                    ocAdapter = ordenCompra.ParearSoloSKU();
                    break;
                case 39:
                    var foodFantasy = new FoodFantasy(pdfReader);
                    ordenCompra = foodFantasy.GetOrdenCompra();
                    ocAdapter = ordenCompra.ParearCentroCostoSinSku();
                    break;
                case 40:
                    var integraMedica = new IntegraMedica(pdfReader);
                    ordenCompra = integraMedica.GetOrdenCompra();
                    ocAdapter = ordenCompra.AdapterGenericFormatWithDescriptionCencosToCompraIntegracion();
                    switch (ocAdapter.RutCli)
                    {
                        case 96845430:
                            ocAdapter.Observaciones += integraMedica.Cc96845430Observaciones[int.Parse(ocAdapter.CenCos)];
                            break;
                        case 96986050:
                            ocAdapter.Observaciones += integraMedica.Cc96986050Observaciones[int.Parse(ocAdapter.CenCos)];
                            break;
                        case 79716500:
                            ocAdapter.Observaciones += integraMedica.Cc79716500Observaciones[int.Parse(ocAdapter.CenCos)];
                            break;
                        case 76098454:
                            ocAdapter.Observaciones += integraMedica.Cc76098454Observaciones[int.Parse(ocAdapter.CenCos)];
                            break;
                    }
                    break;
                case 41:
                    var ecoriles = new Ecoriles(pdfReader);
                    ordenCompra = ecoriles.GetOrdenCompra();
                    ordenCompra.CentroCosto = "2";
                    ocAdapter = ordenCompra.ParearSoloSKU();
                    break;
                case 42:
                    var komatsuCumminsArrienda = new KomatsuCumminsArrienda(pdfReader);
                    ordenCompra = komatsuCumminsArrienda.GetOrdenCompra();
                    ocAdapter = ordenCompra.AdapterGenericFormatWithSkuToCompraIntegracion();
                    foreach (var a in ocAdapter.DetallesCompra)
                    {
                        a.CodigoBodega = 66;
                    }
                    break;

                case 43:
                    var integraMedicaAtencionAmbulatoria = new IntegraMedicaAtencionAmbulatorio(pdfReader);
                    ordenCompra = integraMedicaAtencionAmbulatoria.GetOrdenCompra();
                    ocAdapter = ordenCompra.AdapterGenericFormatWithDescriptionCencosToCompraIntegracion();
                    switch (ocAdapter.RutCli)
                    {
                        case 96845430:
                            ocAdapter.Observaciones += integraMedicaAtencionAmbulatoria.Cc96845430Observaciones[int.Parse(ocAdapter.CenCos)];
                            break;
                        case 96986050:
                            ocAdapter.Observaciones += integraMedicaAtencionAmbulatoria.Cc96986050Observaciones[int.Parse(ocAdapter.CenCos)];
                            break;
                        case 79716500:
                            ocAdapter.Observaciones += integraMedicaAtencionAmbulatoria.Cc79716500Observaciones[int.Parse(ocAdapter.CenCos)];
                            break;
                        case 76098454:
                            ocAdapter.Observaciones += integraMedicaAtencionAmbulatoria.Cc76098454Observaciones[int.Parse(ocAdapter.CenCos)];
                            break;
                    }
                    break;
                case 44:
                    var komatsuRemman = new KomatsuReman(pdfReader);
                    ordenCompra = komatsuRemman.GetOrdenCompra();
                    ocAdapter = ordenCompra.AdapterGenericFormatWithSkuToCompraIntegracion();
                    foreach (var a in ocAdapter.DetallesCompra)
                    {
                        a.CodigoBodega = 66;
                    }
                    break;
                case 45:
                    var komatsuDistribuidoraCummins = new KomatsuDistribuidoraCummins(pdfReader);
                    ordenCompra = komatsuDistribuidoraCummins.GetOrdenCompra();
                    ocAdapter = ordenCompra.AdapterGenericFormatWithSkuToCompraIntegracion();
                    foreach (var a in ocAdapter.DetallesCompra)
                    {
                        a.CodigoBodega = 66;
                    }
                    break;
                case 46:
                    var gepysEstLimitada = new GepysEstLimitada(pdfReader);
                    ordenCompra = gepysEstLimitada.GetOrdenCompra();
                    ocAdapter = ordenCompra.AdapterGenericFormatWithSkuAndNumericCencosToCompraIntegracion();
                    break;
                case 47:
                    var ecoGestion = new Ecogestion(pdfReader);
                    ordenCompra = ecoGestion.GetOrdenCompra();
                    ocAdapter = ordenCompra.AdapterGenericFormatDescripcionitemToCompraIntegracion();
                    break;
                case 48:
                    var consorcioSeguridad = new ConsorcioCompaniaSeguridad(pdfReader);
                    ordenCompra = consorcioSeguridad.GetOrdenCompra();
                    ocAdapter = ordenCompra.TraspasoUltimateIntegracion();
                    break;
                case 49:
                    var megaSalud = new Megasalud(pdfReader);
                    ordenCompra = megaSalud.GetOrdenCompra();
                    ocAdapter = ordenCompra.ToCompraIntegracionSkuCentroCostoDePdf();
                    break;
                case 50:
                    var arauco = new Arauco(pdfReader);
                    ordenCompra = arauco.GetOrdenCompra();
                    ocAdapter = ordenCompra.ParearSoloSKU();
                    break;
                case 51:
                    var kaeferBuildtek = new KaeferBuildtek(pdfReader);
                    ordenCompra = kaeferBuildtek.GetOrdenCompra();
                    //FACTA CC y PAREO DE SKU
                    //ocAdapter = ordenCompra.TraspasoUltimateIntegracion();
                    break;
                case 52:
                    var asesoriasIcyde = new AsesoriasServiciosCapacitacionIcyde(pdfReader);
                    ordenCompra = asesoriasIcyde.GetOrdenCompra();
                    ocAdapter = ordenCompra.AdapterGenericFormatDescripcionitemToCompraIntegracion();
                    break;

                case 53:
                    var intertek = new Intertek(pdfReader);
                    ordenCompra = intertek.GetOrdenCompra();
                    ocAdapter = ordenCompra.TraspasoUltimateIntegracion();
                    break;

                case 54:
                    var analisisAmbientales = new Ecoriles(pdfReader);
                    ordenCompra = analisisAmbientales.GetOrdenCompra();
                    ordenCompra.CentroCosto = "1";
                    ocAdapter = ordenCompra.ParearSoloSKU();
                    break;
                case 55:
                    var afpHabitat = new AFPHabitat(pdfReader);
                    ordenCompra = afpHabitat.GetOrdenCompra();
                    ocAdapter = ordenCompra.TraspasoSinPareo();
                    break;
                case 56:
                    var corporacionDesarrolloTecnologico = new CorporacionDesarrolloTecnologico(pdfReader);
                    ordenCompra = corporacionDesarrolloTecnologico.GetOrdenCompra();
                    ocAdapter = ordenCompra.TraspasoUltimateIntegracion();
                    break;
                case 57:
                    var aguasAndinas = new Ecoriles(pdfReader);
                    ordenCompra = aguasAndinas.GetOrdenCompra();
                    ordenCompra.CentroCosto = "161";
                    ocAdapter = ordenCompra.ParearSoloSKU();
                    break;
                case 58:
                    var ingeProject = new Ingeproject(pdfReader);
                    ordenCompra = ingeProject.GetOrdenCompra();
                    ocAdapter = ordenCompra.TraspasoUltimateIntegracion();
                    break;
                case 59:
                    var sociedadAraucana = new SociedadEducacionalAraucana(pdfReader);
                    ordenCompra = sociedadAraucana.GetOrdenCompra();
                    ocAdapter = ordenCompra.ParearSoloDescripcionCliente();
                    break;
                case 60:
                    var dunkinDonuts = new DunkinDonuts(pdfReader);
                    ordenCompra = dunkinDonuts.GetOrdenCompra();
                    ocAdapter = ordenCompra.TraspasoUltimateIntegracion();
                    break;
                case 61:
                    var teveuk = new Teveuk(pdfReader);
                    ordenCompra = teveuk.GetOrdenCompra();
                    ocAdapter = ordenCompra.TraspasoUltimateIntegracion();
                    break;
                case 62:
                    var colegioCoya = new ColegioCoya(pdfReader);
                    ordenCompra = colegioCoya.GetOrdenCompra();
                    ocAdapter = ordenCompra.TraspasoUltimateIntegracion();
                    break;
                case 63:
                    var sociedadInstruccion = new SociedadInstruccion(pdfReader);
                    ordenCompra = sociedadInstruccion.GetOrdenCompra();
                    ocAdapter = ordenCompra.TraspasoUltimateIntegracion();
                    break;
                case 64:
                    var shawAlmexChile = new ShawAlmexChile(pdfReader);
                    ordenCompra = shawAlmexChile.GetOrdenCompra();
                    ocAdapter = ordenCompra.TraspasoUltimateIntegracion();
                    break;
                case 65:
                    var unitedNations = new UnitedNations(pdfReader);
                    ordenCompra = unitedNations.GetOrdenCompra();
                    ocAdapter = ordenCompra.TraspasoUltimateIntegracion();
                    break;
                case 66:
                    var ingeniericaComercializadoraRiego = new IngenieriaComercializadoraRiego(pdfReader);
                    ordenCompra = ingeniericaComercializadoraRiego.GetOrdenCompra();
                    ocAdapter = ordenCompra.TraspasoUltimateIntegracion();
                    break;

                case 67:
                    var envasadosMovipackChile = new EnvasadosMovipackChile(pdfReader);
                    ordenCompra = envasadosMovipackChile.GetOrdenCompra();
                    ocAdapter = ordenCompra.TraspasoUltimateIntegracion();
                    break;
                case 68:
                    var larrainSalas = new LarrainSalas(pdfReader);
                    ordenCompra = larrainSalas.GetOrdenCompra();
                    ocAdapter = ordenCompra.TraspasoUltimateIntegracion();
                    break;
                case 69:
                    var laboratorioLBC = new LaboratorioLBC(pdfReader);
                    ordenCompra = laboratorioLBC.GetOrdenCompra();
                    //FALTA HOMOLOGACION Y CCC
                    break;
                case 70:
                    var proyektaSA = new ProyektaSA(pdfReader);
                    ordenCompra = proyektaSA.GetOrdenCompra();
                    ocAdapter = ordenCompra.TraspasoUltimateIntegracion();
                    break;
                case 71:
                    var biomedicalDistributionChile = new BiomedicalDistributionChile(pdfReader);
                    ordenCompra = biomedicalDistributionChile.GetOrdenCompra();
                    ocAdapter = ordenCompra.TraspasoUltimateIntegracion();
                    break;
                case 72:
                    var loginsa = new BiomedicalDistributionChile(pdfReader);
                    ordenCompra = loginsa.GetOrdenCompra();
                    ocAdapter = ordenCompra.TraspasoUltimateIntegracion();
                    //IDENTIFICAR CC
                    break;
                case 73:
                    var depositosContenedores = new DepositosContenedores(pdfReader);
                    ordenCompra = depositosContenedores.GetOrdenCompra();
                    ocAdapter = ordenCompra.TraspasoUltimateIntegracion();
                    break;
                case 74:
                    var aridosSantaFerSA = new AridosSantaFeSA(pdfReader);
                    ordenCompra = aridosSantaFerSA.GetOrdenCompra();
                    ocAdapter = ordenCompra.TraspasoUltimateIntegracion();
                    break;
                case 75:
                    var cementosTransex = new CementosTransex(pdfReader);
                    ordenCompra = cementosTransex.GetOrdenCompra();
                    break;
                case 76:
                    var ezentis = new Ezentis(pdfReader);
                    ordenCompra = ezentis.GetOrdenCompra();
                    ocAdapter = ordenCompra.TraspasoUltimateIntegracion();
                    break;
                case 77:
                    var rimasa = new Rimasa(pdfReader);
                    ordenCompra = rimasa.GetOrdenCompra();
                    ocAdapter = ordenCompra.TraspasoUltimateIntegracion();
                    break;
                case 78:
                    var grupoEulenChile = new GrupoEulen(pdfReader);// EULEN CHILE S.A.
                    ordenCompra = grupoEulenChile.GetOrdenCompra();
                    ocAdapter = ordenCompra.TraspasoUltimateIntegracion();
                    break;
                case 79:
                    var eluenChile = new EulenChile(pdfReader);
                    ordenCompra = eluenChile.GetOrdenCompra();
                    ocAdapter = ordenCompra.TraspasoUltimateIntegracion();
                    break;
                case 80:
                    var petrobrasChileRed = new Petrobras(pdfReader);
                    ordenCompra = petrobrasChileRed.GetOrdenCompra();
                    ordenCompra.Rut = "79706120";
                    ocAdapter = ordenCompra.TraspasoUltimateIntegracion();
                    break;
                case 81:
                    var petrobrasChileDistribucion = new Petrobras(pdfReader);
                    ordenCompra = petrobrasChileDistribucion.GetOrdenCompra();
                    ordenCompra.Rut = "79588870";
                    ocAdapter = ordenCompra.TraspasoUltimateIntegracion();
                    break;
                case 82:
                    var eulenSeugirdad = new GrupoEulen(pdfReader);
                    ordenCompra = eulenSeugirdad.GetOrdenCompra();
                    ocAdapter = ordenCompra.TraspasoUltimateIntegracion();
                    break;
                case 83:
                    var servicioExternosACH = new Esach(pdfReader);
                    ordenCompra = servicioExternosACH.GetOrdenCompra();
                    ocAdapter = ordenCompra.TraspasoUltimateIntegracion();
                    break;
                case 84:
                    var report = new Report(pdfReader);
                    ordenCompra = report.GetOrdenCompra();
                    break;
                case 85:
                    var tecnologiaTransporteMinerales = new TecnologiaTransporteMinerales(pdfReader);
                    ordenCompra = tecnologiaTransporteMinerales.GetOrdenCompra();
                    ocAdapter = ordenCompra.TraspasoUltimateIntegracion();
                    break;










                case 200:
                    var cerveceriaCCU = new CerveceriaCCU(pdfReader);
                    ordenCompra =  cerveceriaCCU.GetOrdenCompra();
                    ocAdapter = ordenCompra.TraspasoUltimateIntegracion();
                    break;

                case 201:
                    var constructoraStaFe = new ConstructoraStaFe(pdfReader);
                    ordenCompra = constructoraStaFe.GetOrdenCompra();
                    ocAdapter = ordenCompra.TraspasoUltimateIntegracion();
                    break;
                case 202:
                    var seidorChile = new SeidorChile(pdfReader);
                    ordenCompra = seidorChile.GetOrdenCompra();
                    ocAdapter = ordenCompra.TraspasoUltimateIntegracion();
                    break;

                case 203:
                    var cftSanAgustin = new CFTSanAgustin(pdfReader);
                    ordenCompra = cftSanAgustin.GetOrdenCompra();
                    ocAdapter = ordenCompra.TraspasoUltimateIntegracion();
                    break;
                case 204:
                    var alimentosSanMartin = new AlimentosSanMartin(pdfReader);
                    ordenCompra = alimentosSanMartin.GetOrdenCompra();
                    ocAdapter = ordenCompra.TraspasoUltimateIntegracion();
                    break;
                case 205:
                    var zical = new Zical(pdfReader);
                    ordenCompra = zical.GetOrdenCompra();
                    ocAdapter = ordenCompra.TraspasoUltimateIntegracion();
                    break;
                case 206:
                    var clubAereoCarabineros = new ClubAereoCarabineros(pdfReader);
                    ordenCompra = clubAereoCarabineros.GetOrdenCompra();
                    ocAdapter = ordenCompra.TraspasoUltimateIntegracion();
                    break;
            }
            ExecutePostProcess(option, pdfReader, ordenCompra, ocAdapter, ocAdapterList);
        }


        #region ReaderPdfOrderFromRootDirectory
        public static void ReadPdfOrderFromRootDirectory()
        {
            OnlyOne = false;
            if (!OracleDataAccess.TestConexion()) return;
            if (InternalVariables.CountCriticalError == 3)
            {
                if (InternalVariables.CountSendErrorAlert != 0) return;
                InternalVariables.CountSendErrorAlert++;
                SendAlertError();
                return;
            }
            InitializerAnalysis();
            /*
             *  PDF de Carpeta 'Procesar'
             */
            var count = 0;
            foreach (var pdfReader in Directory
                .GetFiles(@InternalVariables.GetOcAProcesarFolder(), "*.pdf").
                Select(pdfPath => new PDFReader(pdfPath))
                .Where(pdfReader => !pdfReader.PdfFileName.Contains("OCDistribucion_Nro_")
                        && 
                        !(pdfReader.PdfFileName.Contains("OC_Nro_") 
                        && pdfReader.PdfFileName.Contains("_Resumen.pdf"))))
                        //&& !pdfReader.PdfFileName.Contains("<<<<<<<")))
            {
                try
                {
                    ExecuteSinglePdf(pdfReader);
                    count++;
                }
                catch (Exception e)
                {
                    ThrowAnalysisError(pdfReader.PdfFileName, e);
                }
            }

            /*
             *  CLINICA DAVILA - ANEXOS
             */
            foreach (var pdfReader in (from pdfPath in Directory
                .GetFiles(@InternalVariables.GetOcDavilaAnexoAProcesarFolder(), "*.pdf")
                                       let pdfReader = new PDFReader(pdfPath)
                                       let onlyOneLine = pdfReader.ExtractTextFromPdfToString()
                                       where !onlyOneLine.Contains(InternalVariables.PdfFormats[-8])
                                       select pdfPath).
                Select(pdfPath => new PDFReader(pdfPath)))
            {
                try
                {
                    ExecuteSinglePdf(pdfReader);
                    count++;
                }
                catch (Exception e)
                {
                    ThrowAnalysisError(pdfReader.PdfFileName, e);
                }
            }

            /*
             *  EXCEL
             */
            foreach (var xls in Directory.GetFiles(InternalVariables.GetOcExcelAProcesarFolder(), "*.xls"))
            {
                var pdfOutput = xls.Replace(".xls", ".pdf");
                Console.WriteLine(xls);
                Console.WriteLine(pdfOutput);
                MainConverter.SaveExcelToPdf(xls, pdfOutput);
                DebugAnalizarXls(pdfOutput);
                //MoveFileToProcessFolder(xls);
                DeleteFile(pdfOutput);
                count++;
            }
            foreach (var xls in Directory.GetFiles(InternalVariables.GetOcExcelAProcesarFolder(), "*.xlsx"))
            {
                var pdfOutput = xls.Replace(".xlsx", ".pdf");
                Console.WriteLine(xls);
                Console.WriteLine(pdfOutput);
                MainConverter.SaveExcelToPdf(xls, pdfOutput);
                DebugAnalizarXls(pdfOutput);
                //MoveFileToProcessFolder(xls);
                DeleteFile(pdfOutput);
                count++;
            }
            FinishAnalysis(count);
        }


        #endregion

        public static void ReadExcelOrderFromRootDirectory(string xlsPath)
        {
            var excelReader = new ExcelReader(xlsPath);
            var excelRawText = excelReader.ExtractTextLikePdfRaw();
            Console.WriteLine("========================\nRAW LINE\n========================\n" + excelRawText+"\n ========================\n\n\n");
            var option = GetXlsOptionNumber(excelRawText);
            OrdenCompra ordenCompra = null;
            var ocAdapter = new OrdenCompraIntegracion();
            //var ocAdapterList = new List<OrdenCompraIntegracion>();
            Console.WriteLine($"FIRST: {option}");
            switch (option)
            {
                case 0:
                    var flores = new FloresExcel(excelReader);
                    //foreach (var line in flores.GetHojaArrayLines().SelectMany(hoja => hoja))
                    //{
                    //    Console.WriteLine(line);
                    //}
                    //var separador = "====================================";
                    //Console.WriteLine($"{separador}\n{separador}\n");
                    //foreach (var fila in flores.GetHojaMatizLines().SelectMany(hoja => hoja))
                    //{
                    //    foreach (var columna in fila)
                    //    {
                    //        Console.Write($" {columna}");
                    //    }
                    //    Console.WriteLine("");
                    //}
                    ordenCompra = flores.GetOrdenCompra();
                    //ocAdapter = ordenCompra.AdapterFloresToCompraIntegracion();
                    break;
                case 2:
                    var f = new FloresExcel(excelReader);
                    foreach (var line in f.GetHojaArrayLines().SelectMany(hoja => hoja))
                    {
                        Console.WriteLine(line);
                    }
                    var separador = "====================================";
                    Console.WriteLine($"{separador}\n{separador}\n");
                    foreach (var fila in f.GetHojaMatizLines().SelectMany(hoja => hoja))
                    {
                        foreach (var columna in fila)
                        {
                            Console.Write($" {columna}");
                        }
                        Console.WriteLine("");
                    }
                    break;
            }
            if (InternalVariables.IsDebug())
            {
                Console.WriteLine("OC:\n" + ordenCompra);
            }
            if (option != -1)
            {
                if (!OracleDataAccess.InsertOrdenCompraIntegración(ocAdapter))
                {
                    //TODO THROW INSERT ERROR
                }
            }
            else
            {
                //TODO THROW FORMAT ERROR, MOVE FILE TO SPECIFIC FOLDER
            }
        }


        #region ExecutePostProcess

        private static void ExecutePostProcess(int option, PDFReader pdfReader, OrdenCompra ordenCompra, OrdenCompraIntegracion ocAdapter, List<OrdenCompraIntegracion> ocAdapterList)
        {
            if (InternalVariables.IsDebug())
            {
                Console.WriteLine($"PostProcess:{option} \nOC:\n" + ordenCompra);
            }
            if (option != -1)
            {
                if (option == 4 || option == 10
                    || option == 12 
                    || option == -8 
                    || option == 21 || option == 22 
                    || option == 31)
                {
                    var totalResult = true;
                    foreach (var ocIntegracion in ocAdapterList)
                    {
                        if (!OracleDataAccess.InsertOrdenCompraIntegración(ocIntegracion))
                        {
                            totalResult = false;
                        }
                    }
                    if (!totalResult)
                    {
                        ThrowInsertError(pdfReader.PdfFileName);
                    }
                    else
                    {
                        if (ordenCompra != null)
                        {
                            Log.Save($"Orden N°: {ordenCompra.NumeroCompra}, " +
                                     $"de: {InternalVariables.PdfFormats[option == -8 ? 8 : option]}, " +
                                     "procesada exitosamente...");
                            foreach (var ocIntegracion in ocAdapterList)
                                Log.AddMailUpdateTelemarketing(ocIntegracion);
                            MoveFileToProcessFolder(pdfReader.PdfPath, ordenCompra);
                        }
                        else if (option == 21 || option == 22 || option == 31 || option == -8)
                        {
                            //Console.WriteLine("===================================0005");
                            ordenCompra = new OrdenCompra {Rut = ocAdapterList[0].RutCli.ToString()};
                            MoveFileToProcessFolder(pdfReader.PdfPath, ordenCompra);
                            foreach (var ocIntegracion in ocAdapterList)
                            {
                                Log.Save($"Orden N°: {ocIntegracion.OcCliente}, " +
                                    $"de: {InternalVariables.PdfFormats[option == -8 ? 8 : option]}, " +
                                    "procesada exitosamente...");
                                Log.AddMailUpdateTelemarketing(ocIntegracion);
                            }

                        }
                    }
                }
                else
                {
                    if (OracleDataAccess.InsertOrdenCompraIntegración(ocAdapter))
                    {
                        MoveFileToProcessFolder(pdfReader.PdfPath, ordenCompra);
                        if (option == 28)
                        {
                            if (File.Exists(pdfReader.PdfPath.Replace("OC_Nro", "OCDistribucion_Nro")))
                            {
                                MoveFileToProcessFolder(pdfReader.PdfPath.Replace("OC_Nro", "OCDistribucion_Nro"), ordenCompra);
                            }
                            else if (File.Exists(pdfReader.PdfPath.Replace(".pdf", "_Resumen.pdf")))
                            {
                                MoveFileToProcessFolder(pdfReader.PdfPath.Replace(".pdf", "_Resumen.pdf"), ordenCompra);
                            }
                            
                        }
                        Log.Save($"Orden N°: {ocAdapter.OcCliente}, " +
                                 $"de: {InternalVariables.PdfFormats[option]}, " +
                                 "procesada exitosamente...");
                        Log.AddMailUpdateTelemarketing(ocAdapter);

                    }
                    else
                    {
                        ThrowInsertError(pdfReader.PdfFileName);
                    }
                }
            }
            else
            {
                var stringPath = pdfReader.PdfPath;
                var stringFileName = pdfReader.PdfFileName;
                ThrowFormatError(stringFileName, stringPath);
                MoveFileToProcessFolder(stringPath, ordenCompra);
            }
        }

        private static void ExecuteSingleXls(PDFReader pdfReader)
        {
            var first = GetXlsOptionNumber(pdfReader.ExtractTextFromPdfToString());
            OrdenCompra ordenCompra = null;
            var ocAdapter = new OrdenCompraIntegracion();
            var ocAdapterList = new List<OrdenCompraIntegracion>();
            Console.WriteLine($"FIRST: {first}");
            switch (first)
            {
                case 0:
                    var flores = new Flores(pdfReader);
                    ordenCompra = flores.GetOrdenCompra();
                    ocAdapter = ordenCompra.AdapterFloresToCompraIntegracion();
                    break;
                //case 1:
                //    var planillaEstandar = new PlanillaEstandar(pdfReader);
                //    planillaEstandar.GetOrdenCompra();

                //    break;
            }
            if (InternalVariables.IsDebug())
            {
                Console.WriteLine("OC:\n" + ordenCompra);
            }
            if (first != -1)
            {
                if (!OracleDataAccess.InsertOrdenCompraIntegración(ocAdapter))
                {
                    ThrowInsertError(pdfReader.PdfFileName);
                }
            }
            else
            {
                ThrowFormatError(pdfReader.PdfFileName, pdfReader.PdfPath);
                MoveFileToProcessFolder(pdfReader.PdfPath, ordenCompra);
            }
        }

        #endregion


        #region GetOption PDF/Excel
        /// <summary>
        /// Optiene el Numero de la Empresa a Procesar
        /// </summary>
        /// <param name="pdfReader">PDF Reader</param>
        /// <returns>Número Opción Empresa</returns>
        private static int GetOptionNumber(PDFReader pdfReader)
        {
            var onlyOneLine = pdfReader.ExtractTextFromPdfToString();
            var first = -1;
            foreach (var form in InternalVariables.PdfFormats)
            {
                if (form.Value.Contains(";"))
                {
                    var split = form.Value.Split(';');
                    var match = split.Count(sp => onlyOneLine.Contains(sp));
                    if (match == split.Count())
                    {
                        first = form.Key;
                        break;
                    }
                    //if (onlyOneLine.Contains(split[0]) &&
                    //    onlyOneLine.Contains(split[1]))
                    //{
                    //    first = form.Key;
                    //    break;
                    //}
                }else if (form.Value.Contains(":"))
                {
                    var split = form.Value.Split(':');
                    if (split.Any(sp => onlyOneLine.Contains(sp)))
                    {
                        first = form.Key;
                    }
                }else if (onlyOneLine.Contains(form.Value))
                {
                    first = form.Key;
                    break;
                }
            }
            //foreach (var format in PdfFormats.Where(format => onlyOneLine.Contains(format.Value)))
            //{
            //    first = format.Key;
            //    break;
            //}
            if (first == -1)
            {
                try
                {
                    onlyOneLine = onlyOneLine.DeleteNullHexadecimalValues();
                    foreach (var format in InternalVariables.PdfFormats.Where(format => onlyOneLine.Contains(format.Value)))
                    {
                        first = format.Key;
                        break;
                    }
                }
                catch (ArgumentOutOfRangeException)
                {
                    first = -1;
                }
            }
            return first;
        }


        private static int GetXlsOptionNumber(string onlyOneLine)
        {
            //var onlyOneLine = pdfReader.ExtractTextFromPdfToString();
            var first = -1;
            foreach (var form in InternalVariables.XlsFormat)
            {
                if (form.Value.Contains(";"))
                {
                    var split = form.Value.Split(';');
                    if (onlyOneLine.Contains(split[0]) &&
                        onlyOneLine.Contains(split[1]))
                    {
                        first = form.Key;
                        break;
                    }
                }else if (form.Value.Contains("*"))
                {
                    var split = form.Value.Split('*');
                    if (split.Any(op => onlyOneLine.Contains(op)))
                    {
                        first = form.Key;
                    }
                }
                if (onlyOneLine.Contains(form.Value))
                {
                    first = form.Key;
                    break;
                }
            }
            //foreach (var format in PdfFormats.Where(format => onlyOneLine.Contains(format.Value)))
            //{
            //    first = format.Key;
            //    break;
            //}
            if (first == -1)
            {
                try
                {
                    onlyOneLine = onlyOneLine.DeleteNullHexadecimalValues();
                    foreach (var format in InternalVariables.PdfFormats.Where(format => onlyOneLine.Contains(format.Value)))
                    {
                        first = format.Key;
                        break;
                    }
                }
                catch (ArgumentOutOfRangeException)
                {
                    first = -1;
                }
            }
            return first;
        }

        #endregion


        #region Debug

        public static void DebugAnalizar(string pdfPath)
        {
            //UpdateTelemarketing(1);
            OnlyOne = true;
            ExecuteSinglePdf(new PDFReader(pdfPath));
        }

        public static void DebugAnalizarXls(string pdfPath)
        {
            OnlyOne = true;
            ExecuteSingleXls(new PDFReader(pdfPath));

        }

        #endregion


        #region ThrowMessages


        private static void SendAlertError()
        {
            Email.SendEmailFromProcesosXmlDimerc(
                InternalVariables.GetMainEmailDebug(),
                null,
                "Alerta de Error Consecutivo",
                "Han ocurrido más de 3 errores consecutivos con " +
                $"el archivo: {InternalVariables.LastPdfPathError}. " +
                "Por motivos de seguridad, el sistema no seguirá procesando los ficheros.");
        }

        private static void FinishAnalysis(int count)
        {
            if (count == 0)
            {
                IntegracionPdf.Instance.ShowBalloon("Información",
                    "No existen Ordenes para Procesar...", BalloonIcon.Info);
            }
            else
            {
                //if (!InternalVariables.IsDebug())
                //{
                    UpdateTelemarketing();
                //}
                Log.Save($"Análisis Terminado. Total de Ordenes Procesadas: {count}");
                IntegracionPdf.Instance.ShowBalloon("Información", "Análisis Terminado", BalloonIcon.Info);
                OracleDataAccess.CloseConexion();
            }
            NotifyIconViewModel.SetCanProcessOrderCommand();
        }

        private static void UpdateTelemarketing()
        {
           Log.SendMails();
        }
        
        private static void ThrowFormatError(string pdfFileName, string pdfPath)
        {
            Log.Save($"El formato del PDF: {pdfFileName}, " +
                         "no es posible" +
                     " reconocerlo...");
            Email.SendEmailFromProcesosXmlDimercWithAttachments(
               InternalVariables.GetMainEmailDebug(),
                null,
                "Formato de PDF Desconocido", $"El Formato del PDF: {pdfFileName}, " +
                                       "no es posible reconocerlo, favor de Revisar...",
                new []
                {
                    pdfPath
                });
        }

        private static void ThrowInsertError(string pdfFileName)
        {

            IntegracionPdf.Instance.ShowBalloon("Error",
                "Ha ocurrido un error al momento de insertar " +
                $"las Ordenes de Compra del archivo {pdfFileName}",
                BalloonIcon.Error);
            Log.Save("Error",
                "Ha ocurrido un error al momento de insertar " +
                $"las Ordenes de Compra del archivo {pdfFileName}");
            Email.SendEmailFromProcesosXmlDimerc(
                InternalVariables.GetMainEmailDebug(),
                null,
                "Error de Inserción de Datos",
                "Ha ocurrido un error al momento de insertar " +
                $"las Ordenes de Compra del archivo {pdfFileName}");
        }

        private static void ThrowAnalysisError(string pdfFileName, Exception e)
        {
            Log.Save("Ha ocurrio un error en el analisis de las Ordenes de Compra...");
            Log.Save($"Archivo de Orden de Compra: {pdfFileName}");
            IntegracionPdf.Instance.ShowBalloon("Error",
                "Ha ocurrido un error al momento de Procesar " +
                $"el archivo: {pdfFileName}, " +
                "para mayor información, revisar el archivo de Registro de Errores.",
                BalloonIcon.Error);
            Email.SendEmailFromProcesosXmlDimerc(
                InternalVariables.GetMainEmailDebug(),
                null,
                "Error al momento de Procesar",
                "Ha ocurrido un error al momento de Procesar " +
                $"el archivo: {pdfFileName}, " +
                "para mayor información, revisar el archivo de Registro de Errores.");
            Log.TryError(e.Message);
            Log.TryError(e.ToString());
            InternalVariables.AddCountError(pdfFileName);
        }

        private static void InitializerAnalysis()
        {
            IntegracionPdf.Instance.State = IntegracionPdf.AppState.AnalizandoOrdenes;
            Log.Save("Inicializando Análisis de Ordenes de Compra");
            IntegracionPdf.Instance.ShowBalloon("Información",
                "Inicializando Análisis de Ordenes de Compra", BalloonIcon.Info);
        }

#endregion


    }
}
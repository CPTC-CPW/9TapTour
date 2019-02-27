using NineTapTour.Database;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NineTapTour.Models;
using System.Collections;
using System.Collections.ObjectModel;

namespace NineTapTour.Forms
{
    public partial class TournamentStats : Form
    {
        public TournamentStats()
        {
            InitializeComponent();
        }

        private void TournamentStats_Load(object sender, EventArgs e)
        {
            if (!frmMemberScores.selectedTournament.ThreeOutOf4)
            {
                Tournament selectedTournament = new Tournament();
                selectedTournament = frmMemberScores.selectedTournament;
                lblTournamentName.Text = "Tournament ID: (" + selectedTournament.Id + ")\nTournament Location: " + selectedTournament.Location + "\nDate: " + selectedTournament.Date;

                NineTapDb db = new NineTapDb();
                var tournamentStatsList = (from p in db.Participants
                                           join m in db.Members on p.Member.Id equals m.Id
                                           join g in db.Games on p.Game.Id equals g.Id
                                           join t in db.Tournaments on p.Tournament.Id equals t.Id
                                           where t.Id == selectedTournament.Id
                                           orderby (g.Game1 + g.Game2 + g.Game3 + g.Game4) descending
                                           select new
                                           {
                                               p.Member.Number,
                                               p.Member.FirstName,
                                               p.Member.LastName,
                                               p.Squad,
                                               ScratchTotal = ((g.Game1.HasValue ? g.Game1 : 0) + (g.Game2.HasValue ? g.Game2 : 0) + (g.Game3.HasValue ? g.Game3 : 0) + (g.Game4.HasValue ? g.Game4 : 0)),
                                               GameTotal = (((g.Game1.HasValue ? g.Game1 : 0) + (g.Handicap + g.Bonus)) + ((g.Game2.HasValue ? g.Game2 : 0) + (g.Handicap + g.Bonus)) + ((g.Game3.HasValue ? g.Game3 : 0) + (g.Handicap + g.Bonus)) + ((g.Game4.HasValue ? g.Game4 : 0) + (g.Handicap + g.Bonus))),
                                               g.Game1,
                                               g.Game2,
                                               g.Game3,
                                               g.Game4,
                                               p.Game.Handicap,
                                               p.Game.Bonus
                                           }).ToList();
                
                List<TournamentStatsList> statsList = new List<TournamentStatsList>();
                foreach (var item in tournamentStatsList)
                {
                    TournamentStatsList list = new TournamentStatsList
                    {
                        Id = item.Number,
                        FirstName = item.FirstName,
                        LastName = item.LastName,
                        Squad = item.Squad,
                        ScratchTotal = item.ScratchTotal,
                        Top3Scores = item.ScratchTotal + (item.Handicap * 3) + (item.Bonus * 3),
                        Game1 = item.Game1,
                        Game2 = item.Game2,
                        Game3 = item.Game3,
                        Game4 = item.Game4,
                        Handicap = item.Handicap,
                        Bonus = item.Bonus
                    };
                    statsList.Add(list);
                }
                
                TournamentStatsBindingList bindingList = new TournamentStatsBindingList(statsList);
                dgvTournamentStats.DataSource = bindingList;
                dgvTournamentStats.Refresh();
                
            }
            else
            {
                Tournament selectedTournament = new Tournament();
                selectedTournament = frmMemberScores.selectedTournament;
                lblTournamentName.Text = "Tournament ID: (" + selectedTournament.Id + ")\nTournament Location: " + selectedTournament.Location + "\nDate: " + selectedTournament.Date;

                NineTapDb db = new NineTapDb();           

                SqlConnection con = new SqlConnection(GetConnection());
                SqlCommand gameOrder = new SqlCommand();
                gameOrder.Connection = con;
                gameOrder.CommandText = @"SELECT Members.Id, Members.FirstName, Members.LastName, Game1, Game2, Game3, Game4, Games.Handicap, Participants.SquadNumber, Games.Bonus
                                        FROM Games JOIN Participants ON Games.Id = Participants.Game_Id
		                                JOIN Tournaments ON Participants.Tournament_Id = Tournaments.Id
		                                JOIN Members ON Members.Id = Participants.Member_Id                                        
                                        WHERE Tournament_Id = @TID
                                        ORDER BY Members.LastName";

                gameOrder.Parameters.AddWithValue("@TID", selectedTournament.Id);

                try
                {
                    // open connection
                    con.Open();

                    // execute command(query)
                    SqlDataReader reader = gameOrder.ExecuteReader();
                    List<TournamentStatsList> statsList = new List<TournamentStatsList>();

                    // view results
                    while (reader.Read())
                    {
                        TournamentStatsList temp = new TournamentStatsList();
                        temp.Handicap = Convert.ToInt32(reader["Handicap"]);
                        temp.Bonus = Convert.ToInt32(reader["Bonus"]);                      
                        List<int?> scores = new List<int?> { Convert.ToInt32(reader["Game1"]), Convert.ToInt32(reader["Game2"]), Convert.ToInt32(reader["Game3"]), Convert.ToInt32(reader["Game4"]) };

                        List<int> topScores = GetTop3OutOf4(scores);
                        int scratchTotal = 0;

                        for (int i = 0; i < 3; i++)
                        {
                            scratchTotal += topScores[i];
                        }

                        temp.ScratchTotal = scratchTotal;
                        temp.Top3Scores = temp.ScratchTotal + (temp.Handicap * 3) + (temp.Bonus * 3);
                        temp.Id = Convert.ToInt32(reader["Id"]);
                        temp.FirstName = reader["FirstName"].ToString();
                        temp.LastName = reader["LastName"].ToString();
                        temp.Squad = Convert.ToInt32(reader["SquadNumber"]);
                        temp.Game1 = Convert.ToInt32(reader["Game1"]);
                        temp.Game2 = Convert.ToInt32(reader["Game2"]);
                        temp.Game3 = Convert.ToInt32(reader["Game3"]);
                        temp.Game4 = Convert.ToInt32(reader["Game4"]);

                        statsList.Add(temp);
                    }                    
                    
                    TournamentStatsBindingList bindingList = new TournamentStatsBindingList(statsList);
                    dgvTournamentStats.DataSource = bindingList;
                    dgvTournamentStats.Refresh();
                }
                catch (SqlException)
                {
                    
                }
                finally
                {
                    con.Dispose();
                }
            }
        }

        //public void SetSortMode()
        //{
        //    int count = dgvTournamentStats.Columns.Count;
        //    for (int i = 0; i < count; i++)
        //    {
        //        dgvTournamentStats.Columns[i].SortMode = DataGridViewColumnSortMode.Automatic;
        //        //dgvTournamentStats.Sort(dgvTournamentStats.Columns[i], ListSortDirection.Ascending);
        //    }
        //}

        //public void SetSortMode(DataGridView dataGridView)
        //{
        //    int count = dgvTournamentStats.Columns.Count;
        //    for (int i = 0; i < count; i++)
        //    {
        //        dataGridView.Columns[i].SortMode = DataGridViewColumnSortMode.Automatic;
        //        //dgvTournamentStats.Sort(dgvTournamentStats.Columns[i], ListSortDirection.Ascending);
        //    }
        //}

        public static List<int> GetTop3OutOf4(List<int?> scores)
        {            
            List<int> listOfValidScores = new List<int>();
            for(int i = 0; i < scores.Count-1; i++)
            {
                if (scores[i].HasValue)
                {
                    listOfValidScores.Add(scores[i].Value);
                }                
            }

            listOfValidScores.Sort();
            listOfValidScores.Reverse();
            return listOfValidScores;            
        }

        public static string GetConnection()
        {
            return ConfigurationManager.ConnectionStrings["NineTapDbConnection"].ConnectionString;
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            printDialog1.Document = printDocument1;
            if (printDialog1.ShowDialog() == DialogResult.OK)
            {
                printDocument1.Print();
            }
        }

        private void printDocument1_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            Bitmap bm = new Bitmap(this.dgvTournamentStats.Width, this.dgvTournamentStats.Height);
            this.dgvTournamentStats.DrawToBitmap(bm, new Rectangle(0, 0, this.dgvTournamentStats.Width, this.dgvTournamentStats.Height));
            e.Graphics.DrawImage(bm, 0, 0);
        }

    //    private void dgvTournamentStats_ColumnHeaderMouseClick(object sender,
    //                       DataGridViewCellMouseEventArgs e)
    //    {
    //        List<TournamentStatsBindingList> bindingLists = dgvTournamentStats.DataSource as List<TournamentStatsBindingList>;
    //        string col = dgvTournamentStats.Columns[e.ColumnIndex].DataPropertyName;
    //        string order = " ASC";
    //        if (dgvTournamentStats.Tag != null)
    //            order = dgvTournamentStats.Tag.ToString().Contains(" ASC") ? " DESC" : " ASC";

    //        dgvTournamentStats.Tag = col + order;

    //        if (order.Contains(" ASC"))
    //            bindingLists = bindingLists.Sort(new DataGridViewComparer());
    //        else
    //            names = names.OrderByDescending(x => col == "first" ? x.first :
    //                                                 col == "last" ? x.last : x.middle).ToList();

    //        dgvTournamentStats.DataSource = names;
    //    }
    }

    // https://docs.microsoft.com/en-us/dotnet/api/system.data.datatable?view=netframework-4.7.2
    // https://stackoverflow.com/questions/14794470/bind-datatable-data-to-gridview-in-windows-form

    public class DataGridViewComparer : IComparer
    {
        public int Compare(object x, object y)
        {
            DataGridViewRow row1 = (DataGridViewRow)x;
            DataGridViewRow row2 = (DataGridViewRow)y;

            int compareResult = string.Compare(
                (string)row1.Cells[0].Value,
                (string)row2.Cells[0].Value);

            if (compareResult == 0)
            {
                compareResult = ((int)row1.Cells[1].Value)
                    .CompareTo((int)row2.Cells[1].Value);
            }

            return compareResult;
        }
    }

    public partial class TournamentStatsList
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Squad { get; set; }
        public int? ScratchTotal { get; set; }
        public int? Top3Scores { get; set; }
        public int? Game1 { get; set; }
        public int? Game2 { get; set; }
        public int? Game3 { get; set; }
        public int? Game4 { get; set; }
        public int? Handicap { get; set; }
        public int? Bonus { get; set; }
    }

    /**
     * Provides a generic collection that supports data binding and additionally supports sorting.
     * TournamentStatsBindingList impliments CollectionBase and IBindingList to allow for DataGridView sorting.
     * IList -> CollectionBase.List
     * https://10tec.com/articles/sort-datagridview.aspx
     */
    public class TournamentStatsBindingList : CollectionBase, IBindingList
    {        
        private IList _bindingList;
        
        private bool _isSorted;
        private ListSortDirection _sortDirection = ListSortDirection.Ascending;
        private PropertyDescriptor _sortProperty;
        
        private ListChangedEventArgs resetEvent = new ListChangedEventArgs(ListChangedType.Reset, -1);
        private ListChangedEventHandler onListChanged;


        public TournamentStatsBindingList()
        {
            _bindingList = new List<TournamentStatsList>();
            LoadMembers();
        }

        public TournamentStatsBindingList(List<TournamentStatsList> list)
        {
            _bindingList = list;
            LoadMembers();
        }        

        public TournamentStatsList this[int index]
        {
            get
            {
                return (TournamentStatsList)(List[index]);
            }
            set
            {
                List[index] = value;
            }
        }
        
        public void LoadMembers()
        {            
            ReadList();
            OnListChanged(resetEvent);
        }

        private void ReadList()
        {
            foreach (var m in _bindingList)
            {
                List.Add(m);
            }
        }

        public int Add(TournamentStatsList value)
        {
            return List.Add(value);
        }

        public TournamentStatsList AddNew()
        {
            return (TournamentStatsList)((IBindingList)this).AddNew();
        }

        public void Remove(TournamentStatsList value)
        {
            List.Remove(value);
        }

        protected virtual void OnListChanged(ListChangedEventArgs ev)
        {
            if (onListChanged != null)
            {
                onListChanged(this, ev);
            }
        }

        protected override void OnClear()
        {
            foreach (TournamentStatsList c in List)
            {
                throw new NotSupportedException();
            }
        }

        protected override void OnClearComplete()
        {
            OnListChanged(resetEvent);
        }

        protected override void OnInsertComplete(int index, object value)
        {
            OnListChanged(new ListChangedEventArgs(ListChangedType.ItemAdded, index));
        }

        protected override void OnRemoveComplete(int index, object value)
        {
            OnListChanged(new ListChangedEventArgs(ListChangedType.ItemDeleted, index));
        }

        protected override void OnSetComplete(int index, object oldValue, object newValue)
        {
            if (oldValue != newValue)
            {
                OnListChanged(new ListChangedEventArgs(ListChangedType.ItemAdded, index));
            }
        }

        internal void TournamentStatsListChanged(List<TournamentStatsList> list)
        {
            int index = List.IndexOf(list);

            OnListChanged(new ListChangedEventArgs(ListChangedType.ItemChanged, index));
        }

        bool IBindingList.AllowEdit
        {
            get { return true; }
        }

        bool IBindingList.AllowNew
        {
            get { return true; }
        }

        bool IBindingList.AllowRemove
        {
            get { return true; }
        }

        bool IBindingList.SupportsChangeNotification
        {
            get { return true; }
        }

        bool IBindingList.SupportsSearching
        {
            get { return false; }
        }

        bool IBindingList.SupportsSorting
        {
            get { return true; }
        }

        bool IBindingList.IsSorted
        {
            get { return _isSorted; }
        }

        ListSortDirection IBindingList.SortDirection
        {
            get { return _sortDirection; }
        }

        PropertyDescriptor IBindingList.SortProperty
        {
            get { return _sortProperty; }
        }

        public event ListChangedEventHandler ListChanged
        {
            add
            {
                onListChanged += value;
            }
            remove
            {
                onListChanged -= value;
            }
        }

        void IBindingList.ApplySort(PropertyDescriptor property, ListSortDirection direction)
        {
            _sortProperty = property;
            _sortDirection = direction;

            List<TournamentStatsList> list = List.Cast<TournamentStatsList>().ToList();
            if (list is null) return;
            list.Sort(Compare);
            _isSorted = true;
            TournamentStatsListChanged(list);
            OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
        }

        int Compare(TournamentStatsList lhs, TournamentStatsList rhs)
        {
            var result = OnComparison(lhs, rhs);
            //invert if descending
            if (_sortDirection == ListSortDirection.Descending)
                result = -result;
            return result;
        }

        private int OnComparison(TournamentStatsList lhs, TournamentStatsList rhs)
        {
            object lhsValue = lhs == null ? null : _sortProperty.GetValue(lhs);
            object rhsValue = rhs == null ? null : _sortProperty.GetValue(rhs);
            if (lhsValue == null)
            {
                return (rhsValue == null) ? 0 : -1; //nulls are equal
            }

            if (rhsValue == null)
            {
                return 1; //first has value, second doesn't
            }

            if (lhsValue is IComparable)
            {
                return ((IComparable)lhsValue).CompareTo(rhsValue);
            }

            if (lhsValue.Equals(rhsValue))
            {
                return 0; //both are the same
            }

            //not comparable, compare ToString
            return lhsValue.ToString().CompareTo(rhsValue.ToString());
        }

        void IBindingList.RemoveSort()
        {
            _sortDirection = ListSortDirection.Ascending;
            _sortProperty = null;
            _isSorted = false;
        }

        void IBindingList.AddIndex(PropertyDescriptor property)
        {
            throw new NotSupportedException();
        }

        int IBindingList.Find(PropertyDescriptor property, object key)
        {
            throw new NotSupportedException();
        }

        void IBindingList.RemoveIndex(PropertyDescriptor property)
        {
            throw new NotSupportedException();
        }

        object IBindingList.AddNew()
        {
            throw new NotSupportedException();
        }
    }
}

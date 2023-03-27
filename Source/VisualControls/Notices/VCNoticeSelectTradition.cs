using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Fantasy_Kingdoms_Battle
{
    internal sealed class VCNoticeSelectTradition : VCCustomNotice
    {
        public VCNoticeSelectTradition()
        {
            SetNotice(-1, FormMain.Config.Gui48_Tradition, "Новая традиция", "Выберите традицию", Color.Yellow, false);

            CellEntity.Click += CellEntity_Click;
        }

        private void CellEntity_Click(object sender, EventArgs e)
        {
            WindowSelectTradition w = new WindowSelectTradition();
            w.Show();
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CrossbarSwitch
{
    public partial class HelpForm : Form
    {
        public HelpForm()
        {
            InitializeComponent();
        }

        private void HelpForm_Load(object sender, EventArgs e)
        {
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            richTextBox1.ReadOnly = true;
            richTextBox1.SelectionFont = new Font(richTextBox1.Font.FontFamily, 15, FontStyle.Bold);
            richTextBox1.AppendText("Начини за използване на симулатора" + Environment.NewLine + Environment.NewLine);
            richTextBox1.SelectionFont = new Font(richTextBox1.Font.FontFamily, 10, FontStyle.Bold);
            richTextBox1.AppendText("Постъпков режим:" + Environment.NewLine);
            richTextBox1.SelectionFont = new Font(richTextBox1.Font.FontFamily, 10, FontStyle.Regular);
            richTextBox1.AppendText("За да използвате симулатора в постъпков режим, вие трябва да изберете опцията \"Постъпков режим на работа\" най-горе в ляво от менюто." + Environment.NewLine +
                "След това от менюто \"Входни данни\" може да регулирате честотата с която да се генерират заявки и броя тактове нужни за тяхното обслужване от всяка памет." + Environment.NewLine +
                "Последните опции които може да промените размера на мрежата която ще бъде изчертана, това става от двата падащи списъка намиращи се под заглавието \"Размер на мрежата:\"" +
                " първият от които е за броя процесори и втория за броя памети." + Environment.NewLine +
                "Това всички бяха опции с които можете да регулирате симулацията в постъпков режим. След като сте готови натиснете бутона \"Изчертай\", за да начертаете мрежата и " +
                " \"+ 1 такт\", за да започне да работи." + Environment.NewLine +
                "Ако искате да видите изходните резулати на мрежата за 100 такта време може и без да я изчертавате да натиснете бутона \"Изходни резултати\"." + Environment.NewLine + Environment.NewLine);
            richTextBox1.SelectionFont = new Font(richTextBox1.Font.FontFamily, 10, FontStyle.Bold);
            richTextBox1.AppendText("Автоматичен режим:" + Environment.NewLine );
            richTextBox1.SelectionFont = new Font(richTextBox1.Font.FontFamily, 10, FontStyle.Regular);
            richTextBox1.AppendText("Той се разделя на два вида с и без визуализация, като визуализацията може да избере от тикчето в менюто, общото и за двата е опцията " +
                "да се избере размер на мрежа от двата падащи списъка левият за брой процесори и десният за брой памети." + Environment.NewLine + Environment.NewLine);
            richTextBox1.SelectionFont = new Font(richTextBox1.Font.FontFamily, 10, FontStyle.Bold);
            richTextBox1.AppendText("- с визуализация: " );
            richTextBox1.SelectionFont = new Font(richTextBox1.Font.FontFamily, 10, FontStyle.Regular);
            richTextBox1.AppendText(" този режим е еднакъв с постъпковия, с малката разлика че тук няма нужда вие да натискате бутон, за да симулирате настъпването на всеки такт," +
                "като за целта имате опции колко секунди да трае един такт и колко дълго искате да трае симулацията( в тактове) като тя може да тряе от 100 до 50000 такта. За да" +
                "стартирате симулация натиснете зеления бутон за старт. " +
                "Понеже с този вид режим може да ви отнеме доста дълго време докато симулацията която сте избрали приключи можете по всяко време да я спрете с червения бутон за стоп. " + Environment.NewLine + Environment.NewLine);
            richTextBox1.SelectionFont = new Font(richTextBox1.Font.FontFamily, 10, FontStyle.Bold);
            richTextBox1.AppendText("- без визуализация: ");
            richTextBox1.SelectionFont = new Font(richTextBox1.Font.FontFamily, 10, FontStyle.Regular);
            richTextBox1.AppendText("- без визуализация: в този вид автоматичен режим ще бъдат пресметнати изходните резултати спрямо размера на мрежа и времето в тактове нужни за изпълнението" +
                "на една заявка които сте избрали, тук се правят изчисления за всяка от 10-те вероятности които може да се изберат за останалите режими, в този режим след приключването му ще се създадaт и графики за" +
                "средната латентност(");
            richTextBox1.SelectionFont = new Font(richTextBox1.Font.FontFamily, 10, FontStyle.Bold);
            richTextBox1.AppendText("average L");
            richTextBox1.SelectionFont = new Font(richTextBox1.Font.FontFamily, 10, FontStyle.Regular);
            richTextBox1.AppendText(") и средната широчина на лентата на пропускане(");
            richTextBox1.SelectionFont = new Font(richTextBox1.Font.FontFamily, 10, FontStyle.Bold);
            richTextBox1.AppendText("average B");
            richTextBox1.SelectionFont = new Font(richTextBox1.Font.FontFamily, 10, FontStyle.Regular);
            richTextBox1.AppendText("). Симулацията в този режим може да бъде стартирана чрез натискане на зеления старт бутон и бутона \"Изходни резултати.\"" + Environment.NewLine + Environment.NewLine);
            richTextBox1.SelectionFont = new Font(richTextBox1.Font.FontFamily, 15, FontStyle.Bold);
            richTextBox1.AppendText("Легенда" + Environment.NewLine + Environment.NewLine);
            richTextBox1.SelectionFont = new Font(richTextBox1.Font.FontFamily, 10, FontStyle.Bold);
            richTextBox1.AppendText(" - p: ");
            richTextBox1.SelectionFont = new Font(richTextBox1.Font.FontFamily, 10, FontStyle.Regular);
            richTextBox1.AppendText("честота на генерираните заявки" + Environment.NewLine);
            richTextBox1.SelectionFont = new Font(richTextBox1.Font.FontFamily, 10, FontStyle.Bold);
            richTextBox1.AppendText(" - B avr: ");
            richTextBox1.SelectionFont = new Font(richTextBox1.Font.FontFamily, 10, FontStyle.Regular);
            richTextBox1.AppendText("средната широчина на лентата на пропускане" + Environment.NewLine);
            richTextBox1.SelectionFont = new Font(richTextBox1.Font.FontFamily, 10, FontStyle.Bold);
            richTextBox1.AppendText(" - REQ ALL: ");
            richTextBox1.SelectionFont = new Font(richTextBox1.Font.FontFamily, 10, FontStyle.Regular);
            richTextBox1.AppendText("общият брой генерирани заявки" + Environment.NewLine);
            richTextBox1.SelectionFont = new Font(richTextBox1.Font.FontFamily, 10, FontStyle.Bold);
            richTextBox1.AppendText(" - REQ SRV: ");
            richTextBox1.SelectionFont = new Font(richTextBox1.Font.FontFamily, 10, FontStyle.Regular);
            richTextBox1.AppendText("общият брой обслужени заявки" + Environment.NewLine);
            richTextBox1.SelectionFont = new Font(richTextBox1.Font.FontFamily, 10, FontStyle.Bold);
            richTextBox1.AppendText(" - L min: ");
            richTextBox1.SelectionFont = new Font(richTextBox1.Font.FontFamily, 10, FontStyle.Regular);
            richTextBox1.AppendText("минимална латентност" + Environment.NewLine);
            richTextBox1.SelectionFont = new Font(richTextBox1.Font.FontFamily, 10, FontStyle.Bold);
            richTextBox1.AppendText(" - L max: ");
            richTextBox1.SelectionFont = new Font(richTextBox1.Font.FontFamily, 10, FontStyle.Regular);
            richTextBox1.AppendText("максимална латентност" + Environment.NewLine);
            richTextBox1.SelectionFont = new Font(richTextBox1.Font.FontFamily, 10, FontStyle.Bold);
            richTextBox1.AppendText(" - L avr: ");
            richTextBox1.SelectionFont = new Font(richTextBox1.Font.FontFamily, 10, FontStyle.Regular);
            richTextBox1.AppendText("средна латентност" + Environment.NewLine + Environment.NewLine);
            richTextBox1.SelectionFont = new Font(richTextBox1.Font.FontFamily, 15, FontStyle.Bold);
            richTextBox1.AppendText("Относно работата на симулатора" + Environment.NewLine + Environment.NewLine);
            richTextBox1.SelectionFont = new Font(richTextBox1.Font.FontFamily, 10, FontStyle.Regular);
            richTextBox1.AppendText("Симулатора запазва настройките които сте избрали преди да го затворите, за да улесни работата ви с него. " + Environment.NewLine);


        }
    }
}

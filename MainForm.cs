namespace VuchMatSimplex
{
    public partial class Form1 : Form
    {
        private Simplex simplex;
        private List<SaveData> steps;
        private int currentStep;
        private Values value;   
        public Form1()
        {
            InitializeComponent();
            steps = new();
            currentStep = 0;
            simplex = new Simplex();
        }

        private void btnLoadData_Click(object sender, EventArgs e)
        {
            using (var openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Text files (*.txt)|*.txt";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    var filePath = openFileDialog.FileName;
                    try
                    {
                        var lines = File.ReadAllLines(filePath);
                        var firstLineParts = lines[0].Split(' ').Select(int.Parse).ToArray();
                        value = new();
                        value.n = firstLineParts[0]; value.m = firstLineParts[1];
                        var optimizationType = lines[1];
                        if (optimizationType == "min")
                            value.isMaxim = false;
                        else
                            value.isMaxim = true;
                        var coefficient = lines[2].Split(' ').Select(int.Parse).ToList();
                        value.fRaw = coefficient;
                        List<List<int>> As = new();
                        List<int> Bs = new();
                        List<string> znaks = new();
                        for (int i = 3; i < lines.Length; i++)
                        {
                            var parts = lines[i].Split(new[] { '=', '>', '<'}, StringSplitOptions.None);
                            if (parts.Length == 2)
                            {
                                var left1 = parts[0].Split(' ').ToList();
                                left1.RemoveAt(left1.Count - 1);
                                var left = left1.Select(int.Parse).ToList();
                                var right = int.Parse(parts[1]);
                                string znak = "=";
                                if (lines[i].Contains(">"))
                                    znak = ">=";
                                else if (lines[i].Contains("<"))
                                    znak = "<=";
                                As.Add(left);
                                Bs.Add(right);
                                znaks.Add(znak);
                            }
                            else
                            {
                                throw new InvalidDataException($"Неверный формат строки: {lines[i]}");
                            }
                        }
                        value.znak = znaks;
                        value.aRaw = As;
                        value.bRaw = Bs;

                    }
                    catch
                    {
                        MessageBox.Show("Error");
                    }
                }
            }
            UpdateDataGridView();
        }

        private void btnApplyGauss_Click(object sender, EventArgs e)
        {
            simplex = new();
            steps = simplex.Solve(value.n, value.m, value.aRaw, value.bRaw, value.znak, value.fRaw, value.isMaxim);
            if (steps.Count == 0)
            {
                MessageBox.Show("Данная задача решения не имеет");
                value.n = 0;
                value.m = 0;
            }
            UpdateDataGridView();
        }

        private void btnClearData_Click(object sender, EventArgs e)
        {
            steps.Clear();
            currentStep = 0;
            dataGridViewMatrix.Rows.Clear();
            dataGridViewMatrix.Columns.Clear();
        }

        private void numericUpDownStep_ValueChanged(object sender, EventArgs e)
        {
            currentStep = (int)numericUpDownStep.Value;
            UpdateDataGridView();
        }


        private void UpdateDataGridView()
        {
            if (currentStep >= steps.Count) return;

            List<List<string>> values = new();
            var val = steps[currentStep];
            for (int i = 0; i < val.basis.Count; i++)
            {
                values.Add(new());
                values[i].Add(val.basis[i].ToString());
            }
            if (val.basis.Count == 0)
                for (int i = 0; i < val.A.Count; i++)
                {
                    values.Add(new());
                }
            values.Add(new());
            if (currentStep != 0 && currentStep != steps.Count - 1)
                values[values.Count - 1].Add("Δ");
            else
                values[values.Count - 1].Add(value.isMaxim ? "max" : "min");
            values.Add(new());
            if (currentStep != 0 && currentStep != steps.Count - 1)
                values[values.Count - 1].Add("ΔM");
            else
                values[values.Count - 1].Add("");
            for (int i = 0; i < val.A.Count; i++)
            {
                if (currentStep == 0 || currentStep == steps.Count - 1)
                    values[i].Add("");
                for (int j = 0; j < val.A[i].Count; j++)
                {
                    if (val.A[i][j].Item1 == 0)
                        values[i].Add($"0");
                    else if (val.A[i][j].Item2 == 1)
                        values[i].Add($"{val.A[i][j].Item1}");
                    else
                        values[i].Add($"{val.A[i][j].Item1} / {val.A[i][j].Item2}");
                }
                values[i].Add(val.znak[i]);
                if (val.B[i].Item1 == 0)
                    values[i].Add($"0");
                else if (val.B[i].Item2 == 1)
                    values[i].Add($"{val.B[i].Item1}");
                else
                    values[i].Add($"{val.B[i].Item1} / {val.B[i].Item2}");
            }
            for (int i = 0; i < val.delta.Count; i++)
            {
                values[val.basis.Count].Add(val.delta[i].Item1.ToString());
            }
            for (int i = 0; i < val.delta.Count; i++)
            {
                values[val.basis.Count + 1].Add(val.delta[i].Item2.ToString());
            }
            values.Add(new());
            values[values.Count - 1].Add("Function");
            for (int i = 0; i < val.A[0].Count; i++)
            {
                values[values.Count - 1].Add(val.F[i].ToString());
            }
            List<string> names = new List<string>();
            if (currentStep != steps.Count - 1 && currentStep != 0) {
                names.Add("basis");
                for (int i = 0; i < val.A[0].Count - val.k; i++)
                    names.Add($"x{i + 1}");
                for (int i = 0; i < val.k; i++)
                    names.Add($"k{i + 1}");
                names.Add("=");
                names.Add("B");
            }
            else
            {
                names.Add(" ");
                for (int i = 0; i < value.n; i++)
                {
                    names.Add($"x{i + 1}");
                }
                names.Add("=");
                names.Add("B");
            }
            values.Insert(0, names);

            int rows = values.Count;
            int cols = values[0].Count;

            dataGridViewMatrix.Rows.Clear();
            dataGridViewMatrix.Columns.Clear();
            dataGridViewMatrix.ColumnCount = cols;
            dataGridViewMatrix.RowCount = rows;

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    if (values[i].Count == j)
                        break;
                    dataGridViewMatrix.Rows[i].Cells[j].Value = values[i][j];
                }
            }

            numericUpDownStep.Maximum = steps.Count - 1;
            numericUpDownStep.Value = currentStep;
        }
    }
}

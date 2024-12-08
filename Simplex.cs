using Microsoft.VisualBasic.ApplicationServices;
using System;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;

namespace VuchMatSimplex
{
    internal class Simplex
    {
        int n, m; // n - количество переменных  m - количество ограничений
        int NM;
        int k;
        bool error = false;
        double errorSum = 0;
        List<List<(int, int)>> A = new()
        {
            new() {(6, 1), (1, 1), (1, 1)},
            new() {(5, 1), (1, 1), (1, 1)}
        };
        List<(int, int)> B = new() { (96, 1), (11, 1) };
        List<string> znak = new() { "=", ">=" };
        bool isMax = false;
        List<(int, int)> F = new() { (4, 1), (1, 1), (7, 1) };
        List<int> basis = new();
        List<(double, double)> delts;
        List<SaveData> saves;
        private void Solve()
        {
            

            for (int i = 0; i < B.Count; i++)
            {
                if (B[i].Item1 < 0)
                {
                    B[i] = (-B[i].Item1, B[i].Item2);
                    if (znak[i] == ">=")
                        znak[i] = "<=";
                    else if (znak[i] == "<=")
                        znak[i] = ">=";
                    for (int j = 0; j < A[i].Count; j++)
                    {
                        A[i][j] = (-A[i][j].Item1, A[i][j].Item2);
                    }
                }
            }
            for (int i = 0; i < m; i++)
            {
                if (znak[i] != "=")
                {
                    for (int j = 0; j < m; j++)
                    {
                        if (j == i)
                        {
                            if (znak[i] == ">=")
                                A[j].Add((-1, 1));
                            else
                                A[j].Add((1, 1));
                        }
                        else
                            A[j].Add((0, 1));
                        F.Add((0, 1));
                    }
                    znak[i] = "=";
                }
            }
            NM = A[0].Count;
            // Начинаем выбирать базисы
            for (int i = 0; i < m; i++)
                basis.Add(-1);
            for (int i = 0; i < A[0].Count; i++)
            {
                int one = -1;
                bool bad = false;
                for (int j = 0; j < m && !bad; j++)
                {
                    if (A[j][i].Item1 == 1)
                    {
                        if (one != -1)
                            bad = true;
                        one = j;
                    }
                    else if (A[j][i].Item1 != 0)
                        bad = true;
                }
                if (!bad && one != -1)
                {
                    basis[one] = i;
                }
            }
            k = 0;
            for (int i = 0; i < m; i++)
            {
                if (basis[i] == -1)
                {
                    for (int j = 0; j < m; j++)
                    {
                        if (i == j)
                            A[j].Add((1, 1));
                        else
                            A[j].Add((0, 1));
                        F.Add((0, 1));
                    }
                    basis[i] = NM + k; k++;
                }
            }

            F.Add((0, 1));
            bool next = true;
            findDelt();
            Save();

            while (next)
            {
                // Таблица с искусственными базисами

                int col = 0;
                (double, double) maxim = (-100000, -100000);
                next = false;
                if (!isMax)
                {
                    for (int i = 0; i < delts.Count - 1; i++)
                    {
                        if (delts[i].Item2 > maxim.Item2)
                        {
                            maxim = delts[i];
                            col = i;
                        }
                        else if (delts[i].Item2 == maxim.Item2 && delts[i].Item1 > maxim.Item1)
                        {
                            maxim = delts[i];
                            col = i;
                        }

                    }
                }
                else
                {
                    maxim = (100000, 100000);
                    for (int i = 0; i < delts.Count - 1; i++)
                    {
                        if (delts[i].Item2 < maxim.Item2)
                        {
                            maxim = delts[i];
                            col = i;
                        }
                        else if (delts[i].Item2 == maxim.Item2 && delts[i].Item1 < maxim.Item1)
                        {
                            maxim = delts[i];
                            col = i;
                        }

                    }
                }
                if (!isMax)
                {
                    if (maxim.Item2 > 0)
                        next = true;
                    else if (maxim.Item2 == 0 && maxim.Item1 > 0)
                        next = true;
                }
                else
                {
                    if (maxim.Item2 < 0)
                        next = true;
                    else if (maxim.Item2 == 0 && maxim.Item1 < 0)
                        next = true;
                }
                if (next)
                {
                    int row = -1;
                    double del = 100000;
                    for (int i = 0; i < m; i++)
                    {
                        if (A[i][col].Item1 < 0)
                            continue;
                        double d1 = ((double)B[i].Item1 / (double)B[i].Item2) / ((double)A[i][col].Item1 / (double)A[i][col].Item2);
                        if (d1 < del)
                        {
                            del = d1;
                            row = i;
                        }
                    }
                    if (row == -1)
                    {
                        next = false;
                        error = true;
                        break;
                    }
                    basis[row] = col;
                    Gauss(row, col);
                    findDelt();
                    Save();
                }
            }
            if (error)
            {
                return;
            }
            SaveData answer = new();
            answer.znak = new();
            answer.B = new();
            answer.basis = new();
            answer.delta = new();
            List<(int, int)> values = new(NM + k);
            for (int i = 0; i < n; i++)
            {
                values.Add((0, 1));
            }
            for (int i = 0; i < basis.Count; i++)
            {
                if (basis[i] < n)
                    values[basis[i]] = B[i];
            }
            (int, int) ans = (0, 1);
            for (int i = 0; i < values.Count; i++)
            {
                ans = plus(ans, proiz(values[i], F[i]));
            }
            answer.B.Add(ans);
            answer.znak.Add("=");
            var lst = new List<List<(int, int)>>();
            lst.Add(values);
            answer.A = lst;
            answer.F = F.Select(x => x.Item1).ToList();
            saves.Add(answer);
        }
        private void findDelt()
        {
            delts = new(); // Базовая часть и M часть
            for (int i = 0; i < A[0].Count; i++)
            {
                double a = 0, b = 0;
                for (int j = 0; j < m; j++)
                {
                    if (basis[j] >= NM)
                    {
                        if (isMax)
                            b -= (double)A[j][i].Item1 / (double)A[j][i].Item2;
                        else
                            b += (double)A[j][i].Item1 / (double)A[j][i].Item2;
                        errorSum += ((double)A[j][i].Item1 / (double)A[j][i].Item2);
                    }
                    else
                    {
                        a += (double)F[basis[j]].Item1 / (double)F[basis[j]].Item2 * ((double)A[j][i].Item1 / (double)A[j][i].Item2);
                    }
                }
                if (i >= NM)
                {
                    if (isMax)
                        b += 1;
                    else
                        b -= 1;
                }
                else
                {
                    if (i < F.Count)
                        a -= ((double)F[i].Item1 / (double)F[i].Item2);
                }
                delts.Add((a, b));
            }

            double a1 = 0, b1 = 0;
            for (int j = 0; j < m; j++)
            {
                if (basis[j] >= NM)
                {
                    if (isMax)
                        b1 -= (double)B[j].Item1 / (double)B[j].Item2;
                    else
                        b1 += (double)B[j].Item1 / (double)B[j].Item2;
                }
                else
                {
                    a1 += (double)F[basis[j]].Item1 / (double)F[basis[j]].Item2 * ((double)B[j].Item1 / (double)B[j].Item2);
                }
            }
            delts.Add((a1, b1));
        }
        private int _gcd(int a, int b)
        {
            if (b == 0)
                return a;
            else
                return _gcd(b, a % b);
        }
        private int GCD(int a, int b)
        {
            return _gcd(Math.Abs(a), Math.Abs(b));
        }
        private void Gauss(int row, int col)
        {
            Console.WriteLine("");
            (int, int) div = A[row][col];
            for (int i = 0; i < A[0].Count; i++)
            {
                A[row][i] = (A[row][i].Item1 * div.Item2, A[row][i].Item2 * div.Item1);
                int gc = GCD(A[row][i].Item1, A[row][i].Item2);
                A[row][i] = (A[row][i].Item1 / gc, A[row][i].Item2 / gc);
            }
            B[row] = (B[row].Item1 * div.Item2, B[row].Item2 * div.Item1);
            int gcB = GCD(B[row].Item1, B[row].Item2);
            B[row] = (B[row].Item1 / gcB, B[row].Item2 / gcB);
            for (int i = 0; i < m; i++)
            {
                if (i == row)
                    continue;
                (int, int) minus = A[row][col];
                minus = (minus.Item1 * A[i][col].Item1, minus.Item2 * A[i][col].Item2);
                int gc = GCD(minus.Item1, minus.Item2);
                minus = (minus.Item1 / gc, minus.Item2 / gc);
                for (int j = 0; j < A[0].Count; j++)
                {
                    (int, int) minusI = (minus.Item1 * A[row][j].Item1, minus.Item2 * A[row][j].Item2);
                    gc = GCD(minus.Item1, minus.Item2);
                    minusI = (minusI.Item1 / gc, minusI.Item2 / gc);
                    if (A[i][j].Item2 != minusI.Item2)
                    {
                        var AAA = A[i][j];
                        A[i][j] = (A[i][j].Item1 * minusI.Item2, A[i][j].Item2 * minusI.Item2);
                        (int, int) minusDop = (minusI.Item1 * AAA.Item2, minusI.Item2 * AAA.Item2);
                        A[i][j] = (A[i][j].Item1 - minusDop.Item1, minusDop.Item2);
                        gc = GCD(A[i][j].Item1, A[i][j].Item2);
                        A[i][j] = (A[i][j].Item1 / gc, A[i][j].Item2 / gc);
                    }
                    else
                    {
                        A[i][j] = (A[i][j].Item1 - minusI.Item1, minusI.Item2);
                        gc = GCD(A[i][j].Item1, A[i][j].Item2);
                        A[i][j] = (A[i][j].Item1 / gc, A[i][j].Item2 / gc);
                    }
                }
                minus = (minus.Item1 * B[row].Item1, minus.Item2 * B[row].Item2);
                gc = GCD(minus.Item1, minus.Item2);
                minus = (minus.Item1 / gc, minus.Item2 / gc);
                if (B[i].Item2 != minus.Item2)
                {
                    var BBB = B[i];
                    B[i] = (B[i].Item1 * minus.Item2, B[i].Item2 * minus.Item2);
                    (int, int) minusDop = (minus.Item1 * BBB.Item2, minus.Item2 * BBB.Item2);
                    B[i] = (B[i].Item1 - minusDop.Item1, minusDop.Item2);
                    gc = GCD(B[i].Item1, B[i].Item2);
                    B[i] = (B[i].Item1 / gc, B[i].Item2 / gc);
                }
                else
                {
                    B[i] = (B[i].Item1 - minus.Item1, minus.Item2);
                    gc = GCD(B[i].Item1, B[i].Item2);
                    B[i] = (B[i].Item1 / gc, B[i].Item2 / gc);
                }
            }
        }

        public List<SaveData> Solve(int n, int m, List<List<int>> aRaw, List<int> bRaw, List<string> znak, List<int> fRaw, bool isMaxim)
        {
            error = false;
            this.n = n;
            this.m = m;
            A = aRaw.Select(x => x.Select(x => (x, 1)).ToList()).ToList();
            B = bRaw.Select(x => (x, 1)).ToList();
            this.znak = znak;
            F = fRaw.Select(x => (x, 1)).ToList();
            isMax = isMaxim;
            List<(double, double)> delt = new();
            for (int i = 0; i < A[0].Count + 1; i++)
                delt.Add((0, 0));
            delts = delt;
            saves = new();
            Save();
            Solve();
            if (error)
                return new();
            return saves;
        }
        private void Save()
        {
            SaveData saveData1 = new();
            List<List<(int, int)>> Acopy = new();
            for (int i = 0; i < A.Count; i++)
            {
                Acopy.Add(new());
                for (int j = 0; j < A[i].Count; j++)
                {
                    Acopy[i].Add(A[i][j]);
                }
            }
            saveData1.A = Acopy;
            saveData1.B = B.ToList();
            saveData1.k = k;
            saveData1.znak = znak.ToArray().ToList();
            List<int> bs = new();
            for (int i = 0; i < basis.Count; i++)
            {
                bs.Add(basis[i]);
            }
            saveData1.delta = delts;
            saveData1.basis = bs;
            saveData1.F = F.Select(x => x.Item1).ToList();
            saves.Add(saveData1);
        }

        private (int, int) plus((int, int) a, (int, int) b)
        {
            (int, int) a1 = a;
            if (a.Item2 != b.Item2)
            {
                a1 = (a.Item1 * b.Item2, a.Item2 * b.Item2);
                b = (b.Item1 * a.Item2, b.Item2 * a.Item2);
                a1 = (a1.Item1 + b.Item1, a1.Item2);
            }
            else
            {
                a1 = (a1.Item1 + b.Item1, a.Item2);
            }
            int gc = GCD(a1.Item1, a1.Item2);
            a1 = (a1.Item1 / gc,  a1.Item2 / gc);
            return a1;
        }
        private (int, int) proiz((int, int) a, (int, int) b)
        {
            (int, int) bufer = (a.Item1 * b.Item1, a.Item2 * b.Item2);
            int gc = GCD(bufer.Item1, bufer.Item2);
            bufer = (bufer.Item1 / gc, bufer.Item2 / gc);
            return bufer;
        }
    }
}

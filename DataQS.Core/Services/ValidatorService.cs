using System;

public class ValidatorService
{
    #region Radiação
    public static double ConstSa(int day)
    {
        double S0 = 1367; // Constante solar em W/m^2
        double G = 2 * Math.PI * (day - 1) / 365; // Ângulo do dia (G)
        double E0 = 1.000110 + 0.034221 * Math.Cos(G) + 0.001280 * Math.Sin(G) +
                    0.000719 * Math.Cos(2 * G) + 0.000077 * Math.Sin(2 * G); // Fator de correção de excentricidade (E0)
        return S0 * E0;
    }

    public static double CosM0(double lat, double lon, int day, double mit)
    {
        double G = 2 * Math.PI * (day - 1) / 365;
        double te = (0.000075 + 0.001868 * Math.Cos(G) - 0.032077 * Math.Sin(G) -
                     0.014615 * Math.Cos(2 * G) - 0.04089 * Math.Sin(2 * G)) * 229.18;
        double sh = mit + 4 * lon + te;
        double w = (12 - (sh / 60)) * 15 * Math.PI / 180;
        double d = 0.006918 - 0.399912 * Math.Cos(G) + 0.070257 * Math.Sin(G) -
                   0.006758 * Math.Cos(2 * G) + 0.000907 * Math.Sin(2 * G) -
                   0.02697 * Math.Cos(3 * G) + 0.00148 * Math.Sin(3 * G);
        return Math.Sin(d) * Math.Sin(lat) + Math.Cos(d) * Math.Cos(w) * Math.Cos(lat);
    }

    public static int[] GloRadComplete(double glo, double dni, double dif, int day, double lat, double lon, double mit)
    {
        double Sa = ConstSa(day);
        double M0 = CosM0(lat, lon, day, mit);
        int[] status = [0, 0, 0, 0];
        if (glo > -4 && glo < (Sa * 1.5 * Math.Pow(M0, 1.2) + 100))
        {
            status[3] = 9;
            if (glo > -2 && glo < (Sa * 1.2 * Math.Pow(M0, 1.2) + 50))
            {
                status[2] = 9;
                double azs = Math.Acos(M0);
                double soma = dni + (dif * M0);
                if (azs < (75 * Math.PI / 180) && soma > 50)
                {
                    status[1] = (glo / soma >= 0.9 && glo / soma <= 1.1) ? 9 : 2;
                }
                else if (azs < (93 * Math.PI / 180) && azs > (75 * Math.PI / 180) && soma > 50)
                {
                    status[1] = (glo / soma >= 0.85 && glo / soma <= 1.15) ? 9 : 2;
                }
                else
                {
                    status[1] = 5;
                }
            }
            else
            {
                status[2] = 2;
                status[1] = 5;
            }
        }
        else
        {
            status = [0, 5, 5, 2];
        }
        return status;
    }

    public static int[] GloRadIncomplete(double glo, int day, double lat, double lon, double mit)
    {
        double Sa = ConstSa(day);
        double M0 = CosM0(lat, lon, day, mit);
        int[] status = [0, 0, 0, 0];
        if (glo > -4 && glo < (Sa * 1.5 * Math.Pow(M0, 1.2) + 100))
        {
            status[3] = 9;
            if (glo > -2 && glo < (Sa * 1.2 * Math.Pow(M0, 1.2) + 50))
            {
                status[2] = 9;
            }
            else
            {
                status[2] = 2;
            }
        }
        else
        {
            status = [0, 0, 5, 2];
        }
        return status;
    }

    public static int[] DifRadComplete(double glo, double dni, double dif, int day, double lat, double lon, double mit)
    {
        double Sa = ConstSa(day);
        double M0 = CosM0(lat, lon, day, mit);
        int[] status = { 0, 0, 0, 0 };

        if (dif > -4 && dif < (Sa * 0.95 * Math.Pow(M0, 1.2) + 50))
        {
            status[3] = 9;
            if (dif > -2 && dif < (Sa * 0.75 * Math.Pow(M0, 1.2) + 30))
            {
                status[2] = 9;
                double azs = Math.Acos(M0);

                double d;
                if (glo == 0)
                {
                    d = dif == 0 ? 1 : 2;
                }
                else
                {
                    d = dif / glo;
                }

                double a = 75 * (Math.PI / 180);
                double aa = 93 * (Math.PI / 180);

                if (azs < a && glo > 50)
                {
                    status[1] = d < 1.05 ? 9 : 2;
                }
                else if (azs > a && azs < aa && glo > 50)
                {
                    status[1] = d < 1.1 ? 9 : 2;
                }
                else
                {
                    status[1] = 5;
                }
            }
            else
            {
                status[2] = 2;
                status[1] = 5;
            }
        }
        else
        {
            status = new int[] { 0, 5, 5, 2 };
        }

        return status;
    }
    public static int[] DifRadIncomplete(double dif, int day, double lat, double lon, double mit)
    {
        double Sa = ConstSa(day);
        double M0 = CosM0(lat, lon, day, mit);
        int[] status = { 0, 0, 0, 0 };

        if (dif > -4 && dif < (Sa * 0.95 * Math.Pow(M0, 1.2) + 50))
        {
            status[3] = 9;
            if (dif > -2 && dif < (Sa * 0.75 * Math.Pow(M0, 1.2) + 30))
            {
                status[2] = 9;
            }
            else
            {
                status[2] = 2;
            }
        }
        else
        {
            status = new int[] { 0, 0, 5, 2 };
        }

        return status;
    }

    public static int[] DniRadComplete(double glo, double dni, double dif, int day, double lat, double lon, double mit)
    {
        double Sa = ConstSa(day);
        double M0 = CosM0(lat, lon, day, mit);
        int[] status = [0, 0, 0, 0];
        if (dni > -4 && dni < Sa)
        {
            status[3] = 9;
            if (dni > -2 && dni < (Sa * 0.95 * Math.Pow(M0, 0.2) + 10))
            {
                status[2] = 9;
                double dh = glo - dif;
                status[1] = (dh >= ((dni * M0) - 50) && dh <= ((dni * M0) + 50)) ? 9 : 2;
            }
            else
            {
                status[2] = 2;
                status[1] = 5;
            }
        }
        else
        {
            status = [0, 5, 5, 2];
        }
        return status;
    }
    public static int[] DniRadIncomplete(double dni, int day, double lat, double lon, double mit)
    {
        double Sa = ConstSa(day); // Função para calcular Sa com base no dia
        double M0 = CosM0(lat, lon, day, mit); // Função para calcular M0 com base em lat, lon, dia e mit
        int[] status = { 0, 0, 0, 0 };

        if (dni > -4 && dni < Sa)
        {
            status[3] = 9;
            if (dni > -2 && dni < (Sa * 0.95 * Math.Pow(M0, 0.2) + 10))
            {
                status[2] = 9;
            }
            else
            {
                status[2] = 2;
            }
        }
        else
        {
            status = new int[] { 0, 0, 5, 2 };
        }

        return status;
    }

    public static int[] LwRadComplete(double lw, double tp)
    {
        int[] status = [0, 0, 0, 0];
        if (lw > 40 && lw < 700)
        {
            status[3] = 9;
            if (lw > 60 && lw < 500)
            {
                status[2] = 9;
                double tpk = tp + 273;
                if (tpk > 170 && tpk < 350)
                {
                    status[1] = (lw > ((0.4 * 5.67e-8) * Math.Pow(tpk, 4)) && lw < ((0.4 * 5.67e-8) * Math.Pow(tpk, 4) + 25)) ? 9 : 2;
                }
                else
                {
                    status[1] = 5;
                }
            }
            else
            {
                status[2] = 2;
                status[1] = 5;
            }
        }
        else
        {
            status = [0, 5, 5, 2];
        }
        return status;
    }
    public static int[] LwRadIncomplete(double lw)
    {
        int[] status = { 0, 0, 0, 0 };

        if (lw > 40 && lw < 700)
        {
            status[3] = 9;
            if (lw > 60 && lw < 500)
            {
                status[2] = 9;
            }
            else
            {
                status[2] = 2;
            }
        }
        else
        {
            status = new int[] { 0, 0, 5, 2 };
        }

        return status;
    }
    public static int[] ParRad(double par, int day, double lat, double lon, double mit)
    {
        double Sa = ConstSa(day);
        double M0 = CosM0(lat, lon, day, mit);
        int[] status = [0, 0, 0, 0];
        if (par > -4 && par < (2.07 * (Sa * 1.5 * Math.Pow(M0, 1.2) + 100)))
        {
            status[3] = 9;
            if (par > -2 && par < (2.07 * (Sa * 1.2 * Math.Pow(M0, 1.2) + 50)))
            {
                status[2] = 9;
            }
            else
            {
                status[2] = 2;
            }
        }
        else
        {
            status = [0, 5, 5, 2];
        }
        return status;
    }

    public static int[] LuxRad(double lux, int day, double lat, double lon, double mit)
    {
        double Sa = ConstSa(day);
        double M0 = CosM0(lat, lon, day, mit);
        int[] status = [0, 0, 0, 0];
        if (lux > -4 && lux < (0.1125 * (Sa * 1.5 * Math.Pow(M0, 1.2) + 100)))
        {
            status[3] = 9;
            if (lux > -2 && lux < (0.1125 * (Sa * 0.95 * Math.Pow(M0, 1.2) + 50)))
            {
                status[2] = 9;
            }
            else
            {
                status[2] = 2;
            }
        }
        else
        {
            status = [0, 5, 5, 2];
        }
        return status;
    }
    #endregion
    #region Pressão (press)
    public static List<int[]> Pressao(double[] data, double pressMax, double pressMin) // ref col 11
    {
        var vPres = new List<int[]>();
        for (int i = 0; i < data.GetLength(0) - 1; i++)
        {
            int[] status = { 0, 0, 0, 0 };
            double pres = data[i];
            double presm = data[i + 1];

            if (pres <= pressMax && pres >= pressMin)
            {
                status[3] = 9;
                double p = Math.Abs(presm - pres);
                status[2] = p <= 0.5 ? 9 : 2;
            }
            else
            {
                status = new int[] { 0, 0, 5, 2 };
            }
            vPres.Add(status);
        }

        double lastPres = data[data.GetLength(0) - 1];
        if (lastPres <= pressMax && lastPres >= pressMin)
        {
            vPres.Add(new int[] { 0, 0, 0, 9 });
        }
        else
        {
            vPres.Add(new int[] { 0, 0, 5, 2 });
        }
        return vPres;
    }

    public static List<int[]> Pressao2(List<int[]> vPres, double[] data) // ref col 11
    {
        int start = 0;
        int end = start + 60;
        bool fim = true;
        int breakCounter = 0;

        while (fim)
        {
            breakCounter++;
            if (breakCounter == 100000) break;
            if (end > vPres.Count)
            {
                end = vPres.Count - 1;
                start = end - 60;
                fim = false;
            }

            double mx = MaxRange(data, start, end);
            double mn = MinRange(data, start, end);
            double p = mx - mn;

            for (int i = start; i <= end; i++)
            {
                if (p >= 0.1)
                {
                    vPres[i][1] = (vPres[i][2] == 2 || vPres[i][2] == 5) ? 5 : 9;
                }
                else
                {
                    vPres[i][1] = (vPres[i][2] == 2 || vPres[i][2] == 5) ? 5 : 2;
                }
            }

            int pmx = ArgMaxRange(data, start, end);
            int pmn = ArgMinRange(data, start, end);

            if (pmx > pmn)
            {
                start += pmx;
                end = start + 60;
            }
            else
            {
                start += pmn;
                end = start + 60;
            }
        }
        return vPres;
    }
    #endregion
    #region Precipitação (rain)
    public static List<int[]> Rain1(double[] data, double rainMax) // ref col 13
    {
        var vRain = new List<int[]>();
        for (int i = 0; i < data.GetLength(0); i++)
        {
            int[] status = { 0, 0, 0, 0 };
            double rain = data[i];

            if (rain >= 0 && rain <= rainMax)
            {
                status[3] = 9;
            }
            else
            {
                status = new int[] { 0, 0, 5, 2 };
            }
            vRain.Add(status);
        }
        return vRain;
    }

    public static List<int[]> Rain2(List<int[]> vRain, double[] data)
    {
        int start = 0;
        int end = start + 60;
        bool fim = true;
        int breakCounter = 0;

        while (fim)
        {
            breakCounter++;
            if (breakCounter == 100000) break;
            if (end >= vRain.Count)
            {
                end = vRain.Count - 1;
                start = end - 60;
                fim = false;
            }

            double mx = MaxRange(data, start, end);
            double mn = MinRange(data, start, end);
            double p = mx - mn;

            for (int i = start; i < end; i++)
            {
                if (p >= 25)
                {
                    vRain[i][2] = vRain[i][3] == 2 ? 5 : 9;
                }
                else
                {
                    vRain[i][2] = vRain[i][3] == 2 ? 5 : 2;
                }
            }

            int pmx = ArgMaxRange(data, start, end);
            int pmn = ArgMinRange(data, start, end);

            if (pmx > pmn)
            {
                start += pmx;
                end = start + 60;
            }
            else
            {
                start += pmn;
                end = start + 60;
            }
        }
        return vRain;
    }

    public static List<int[]> Rain3(List<int[]> vRain, double[] data)
    {
        int start = 0;
        int end = start + 1440;
        bool fim = true;
        int breakCounter = 0;

        while (fim)
        {
            breakCounter++;
            if (breakCounter == 100000) break;
            if (end >= vRain.Count)
            {
                end = vRain.Count - 1;
                start = end - 1440;
                fim = false;
            }

            double mx = MaxRange(data, start, end);
            double mn = MinRange(data, start, end);
            double p = mx - mn;

            for (int i = start; i <= end; i++)
            {
                if (p >= 100)
                {
                    vRain[i][1] = vRain[i][2] == 2 ? 5 : 9;
                }
                else
                {
                    vRain[i][1] = vRain[i][2] == 2 ? 5 : 2;
                }
            }

            int pmx = ArgMaxRange(data, start, end);
            int pmn = ArgMinRange(data, start, end);

            if (pmx > pmn)
            {
                start += pmx;
                end = start + 1440;
            }
            else
            {
                start += pmn;
                end = start + 1440;
            }
        }
        return vRain;
    }
    #endregion
    #region Umidade (humidity)
    public static List<int[]> Umidade(double[] data)// ref col 10
    {
        var vHumid = new List<int[]>();
        for (int i = 0; i < data.GetLength(0) - 1; i++)
        {
            int[] status = { 0, 0, 0, 0 };
            double humid = data[i];
            double humidm = data[i + 1];

            if (humid <= 100 && humid >= 0)
            {
                status[3] = 9;
                double p = humid == 0 ? 1 : Math.Abs((humidm - humid) / humid);
                status[2] = p <= 0.01 ? 9 : 2;
            }
            else
            {
                status = new int[] { 0, 0, 5, 2 };
            }
            vHumid.Add(status);
        }

        double lastHumid = data[data.GetLength(0) - 1];
        if (lastHumid <= 100 && lastHumid >= 0)
        {
            vHumid.Add(new int[] { 0, 0, 0, 9 });
        }
        else
        {
            vHumid.Add(new int[] { 0, 0, 5, 2 });
        }
        return vHumid;
    }

    public static List<int[]> Umidade2(List<int[]> vHumid, double[] data)
    {
        int start = 0;
        int end = start + 60;
        bool fim = true;
        int breakCounter = 0;

        while (fim)
        {
            breakCounter++;
            if (breakCounter == 100000) break;
            if (end >= vHumid.Count)
            {
                end = vHumid.Count - 1;
                start = end - 60;
                fim = false;
            }

            double mx = MaxRange(data, start, end);
            double mn = MinRange(data, start, end);
            double p = mn == 0 ? 0 : (mx - mn) / mn;

            for (int i = start; i <= end; i++)
            {
                if (p >= 0.01)
                {
                    vHumid[i][1] = vHumid[i][2] == 2 ? 5 : 9;
                }
                else
                {
                    vHumid[i][1] = vHumid[i][2] == 2 ? 5 : 2;
                }
            }

            int pmx = ArgMaxRange(data, start, end);
            int pmn = ArgMinRange(data, start, end);

            if (pmx > pmn)
            {
                start += pmx;
                end = start + 60;
            }
            else
            {
                start += pmn;
                end = start + 60;
            }
        }
        return vHumid;
    }
    #endregion
    #region Temperatura (Temp)
    public static List<int[]> Temp1(double[] data, double tempMax, double tempMin)// ref col 9
    {
        var vTemp = new List<int[]>();
        for (int i = 0; i < data.GetLength(0); i++)
        {
            int[] status = { 0, 0, 0, 0 };
            double temp = data[i];
            if (temp <= tempMax && temp >= tempMin)
            {
                status[3] = 9;
            }
            else
            {
                status[3] = 2;
            }
            vTemp.Add(status);
        }
        return vTemp;
    }

    public static List<int[]> Temp2(List<int[]> vTemp, double[] data)
    {
        int start = 0;
        int end = start + 60;
        bool fim = true;
        int breakCounter = 0;

        while (fim)
        {
            breakCounter++;
            if (breakCounter == 100000) break;
            if (end >= vTemp.Count)
            {
                end = vTemp.Count - 1;
                start = end - 60;
                fim = false;
            }

            double mx = MaxRange(data, start, end);
            double mn = MinRange(data, start, end);
            double p = mx - mn;

            for (int i = start; i <= end; i++)
            {
                vTemp[i][2] = p <= 5 ? (vTemp[i][3] == 2 ? 5 : 9) : (vTemp[i][3] == 2 ? 5 : 2);
            }

            int pmx = ArgMaxRange(data, start, end);
            int pmn = ArgMinRange(data, start, end);

            if (pmx > pmn)
            {
                start += pmx;
                end = start + 60;
            }
            else
            {
                start += pmn;
                end = start + 60;
            }
        }
        return vTemp;
    }

    public static List<int[]> Temp3(List<int[]> vTemp, double[] data)
    {
        int start = 0;
        int end = start + 720; // 12 horas
        bool fim = true;
        int breakCounter = 0;

        while (fim)
        {
            breakCounter++;
            if (breakCounter == 100000) break;
            if (end >= vTemp.Count)
            {
                end = vTemp.Count - 1;
                start = end - 720;
                fim = false;
            }

            double mx = MaxRange(data, start, end);
            double mn = MinRange(data, start, end);
            double p = mx - mn;

            for (int i = start; i <= end; i++)
            {
                vTemp[i][1] = p >= 0.5 ? (vTemp[i][2] == 2 || vTemp[i][3] == 2 ? 5 : 9) : (vTemp[i][2] == 2 || vTemp[i][1] == 2 ? 5 : 2);
            }

            int pmx = ArgMaxRange(data, start, end);
            int pmn = ArgMinRange(data, start, end);

            if (pmx > pmn)
            {
                start += pmx;
                end = start + 720;
            }
            else
            {
                start += pmn;
                end = start + 720;
            }
        }
        return vTemp;
    }
    #endregion
    #region Direção do Vento (wind direction)
    public static List<int[]> WindDir(double[] data)
    {
        var vWdir = new List<int[]>();
        for (int i = 0; i < data.GetLength(0); i++)
        {
            int[] status = { 0, 0, 0, 0 };
            double wdir = data[i];
            status[3] = (wdir >= 0 && wdir <= 360) ? 9 : 2;
            vWdir.Add(status);
        }
        return vWdir;
    }

    public static List<int[]> WindDir2(List<int[]> vWdir, double[] data)
    {
        int start = 0;
        int end = start + 180;
        bool fim = true;
        int breakCounter = 0;

        while (fim)
        {
            breakCounter++;
            if (breakCounter == 100000)
                break;

            if (end >= vWdir.Count)
            {
                end = vWdir.Count - 1;
                start = end - 180;
                fim = false;
            }

            double mx = MaxRange(data, start, end);
            double mn = MinRange(data, start, end);
            double p = mx - mn;

            if (p >= 1) // Level 2
            {
                for (int i = start; i <= end; i++)
                {
                    if (vWdir[i][3] == 2)
                        vWdir[i][2] = 5;
                    else
                        vWdir[i][2] = 9;
                }
            }
            else
            {
                for (int i = start; i <= end; i++)
                {
                    if (vWdir[i][3] == 2)
                        vWdir[i][2] = 5;
                    else
                        vWdir[i][2] = 2;
                }
            }

            int pmx = ArgMaxRange(data, start, end);
            int pmn = ArgMinRange(data, start, end);

            if (pmx > pmn)
            {
                start += pmx;
                end = start + 180;
            }
            else
            {
                start += pmn;
                end = start + 180;
            }
        }
        return vWdir;
    }

    public static List<int[]> WindDir3(List<int[]> vWdir, double[] data)
    {
        int start = 0;
        int end = start + 1080;
        bool fim = true;
        int breakCounter = 0;

        while (fim)
        {
            breakCounter++;
            if (breakCounter == 100000)
                break;

            if (end >= vWdir.Count)
            {
                end = vWdir.Count - 1;
                start = end - 1080;
                fim = false;
            }

            double mx = MaxRange(data, start, end);
            double mn = MinRange(data, start, end);
            double p = mx - mn;

            if (p >= 10) // Level 3
            {
                for (int i = start; i <= end; i++)
                {
                    if (vWdir[i][2] == 2 || vWdir[i][3] == 2)
                        vWdir[i][1] = 5;
                    else
                        vWdir[i][1] = 9;
                }
            }
            else
            {
                for (int i = start; i <= end; i++)
                {
                    if (vWdir[i][2] == 2 || vWdir[i][3] == 2)
                        vWdir[i][1] = 5;
                    else
                        vWdir[i][1] = 2;
                }
            }

            int pmx = ArgMaxRange(data, start, end);
            int pmn = ArgMinRange(data, start, end);

            if (pmx > pmn)
            {
                start += pmx;
                end = start + 1080;
            }
            else
            {
                start += pmn;
                end = start + 1080;
            }
        }
        return vWdir;
    }
    #endregion
    #region Velocidade do Vento (Wind Speed)
    public static List<int[]> WindSp(double[] data)
    {
        var vWsp = new List<int[]>();
        for (int i = 0; i < data.GetLength(0); i++)
        {
            int[] status = { 0, 0, 0, 0 };
            double wsp = data[i];
            status[3] = wsp <= 25 && wsp >= 0 ? 9 : 2;
            vWsp.Add(status);
        }
        return vWsp;
    }

    public static List<int[]> WindSp2(List<int[]> vWsp, double[] data)
    {
        int start = 0;
        int end = start + 180;
        bool fim = true;
        int breakCounter = 0;

        while (fim)
        {
            breakCounter++;
            if (breakCounter == 100000) break;
            if (end >= vWsp.Count)
            {
                end = vWsp.Count - 1;
                start = end - 180;
                fim = false;
            }

            double mx = MaxRange(data, start, end);
            double mn = MinRange(data, start, end);
            double p = mx - mn;

            for (int i = start; i <= end; i++)
            {
                vWsp[i][2] = p >= 0.1 ? (vWsp[i][3] == 2 ? 5 : 9) : (vWsp[i][3] == 2 ? 5 : 2);
            }

            int pmx = ArgMaxRange(data, start, end);
            int pmn = ArgMinRange(data, start, end);

            if (pmx > pmn)
            {
                start += pmx;
                end = start + 180;
            }
            else
            {
                start += pmn;
                end = start + 180;
            }
        }
        return vWsp;
    }

    public static List<int[]> WindSp3(List<int[]> vWsp, double[] data)
    {
        int start = 0;
        int end = start + 720;
        bool fim = true;
        int breakCounter = 0;

        while (fim)
        {
            breakCounter++;
            if (breakCounter == 100000) break;
            if (end >= vWsp.Count)
            {
                end = vWsp.Count - 1;
                start = end - 720;
                fim = false;
            }

            double mx = MaxRange(data, start, end);
            double mn = MinRange(data, start, end);
            double p = mx - mn;

            for (int i = start; i <= end; i++)
            {
                vWsp[i][1] = p >= 0.5 ? (vWsp[i][2] == 2 || vWsp[i][3] == 2 ? 5 : 9) : (vWsp[i][2] == 2 || vWsp[i][3] == 2 ? 5 : 2);
            }

            int pmx = ArgMaxRange(data, start, end);
            int pmn = ArgMinRange(data, start, end);

            if (pmx > pmn)
            {
                start += pmx;
                end = start + 720;
            }
            else
            {
                start += pmn;
                end = start + 720;
            }
        }
        return vWsp;
    }
    #endregion
    #region Helper methods for max, min, argmax, argmin within range
    public static double MaxRange(double[] data, int start, int end)
    {
        double max = data[start];
        for (int i = start; i < end; i++) max = Math.Max(max, data[i]);
        return max;
    }

    public static double MinRange(double[] data, int start, int end)
    {
        double min = data[start];
        for (int i = start; i < end; i++) min = Math.Min(min, data[i]);
        return min;
    }

    public static int ArgMaxRange(double[] data, int start, int end)
    {
        int idx = start;
        double max = data[start];
        for (int i = start + 1; i <= end; i++)
        {
            if (data[i] > max)
            {
                max = data[i];
                idx = i;
            }
        }
        return idx - start;
    }

    public static int ArgMinRange(double[] data, int start, int end)
    {
        int idx = start;
        double min = data[start];
        for (int i = start + 1; i <= end; i++)
        {
            if (data[i] <= min)
            {
                min = data[i];
                idx = i;
            }
        }
        return idx - start;
    }
    #endregion
}

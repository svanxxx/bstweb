using System;
using System.Collections.Generic;
using System.Drawing;

public class Channel
{
	readonly List<string> _class = new List<string>();
	readonly List<string> _info = new List<string>();
	readonly List<PointF> _data = new List<PointF>();
	double _xmin = double.MaxValue;
	double _xmax = double.MinValue;
	double _ymin = double.MaxValue;
	double _ymax = double.MinValue;
	double _startxpercent = 0.0;
	double _endxpercent = 100.0;
	public Channel(double startxpercent, double endxpercent)
	{
		_startxpercent = startxpercent;
		_endxpercent = endxpercent;
	}
	public int Add(float x, float y, string info = "", string cssclass = "")
	{
		_data.Add(new PointF(x, y));
		_info.Add(info);
		_class.Add(cssclass);

		_xmax = Math.Max(_xmax, x);
		_xmin = Math.Min(_xmin, x);

		_ymax = Math.Max(_ymax, y);
		_ymin = Math.Min(_ymin, y);

		return (count - 1);
	}
	public int count
	{
		get
		{
			return _data.Count;
		}
	}
	public string getPointInfo(int i)
	{
		return _info[i];
	}
	public string getPointCss(int i)
	{
		return _class[i];
	}
	public double getScreenPtXAt(int i)
	{
		return (_startxpercent + (_endxpercent - _startxpercent) * (_data[i].X - _xmin) / (_xmax - _xmin));
	}
	public double getScreenPtYAt(int i)
	{
		return (_startxpercent + (_endxpercent - _startxpercent) * (_data[i].Y - _ymax) / (_ymin - _ymax));
	}
	public string getLabelXAt(int i)
	{
		return DateTime.FromOADate(_data[i].X).ToShortDateString();
	}
	public string getLabelYAt(int i)
	{
		return _data[i].Y.ToString("0.00");
	}
	public string getLabelXFor(double frac)
	{
		double val = _xmin + (_xmax - _xmin) * frac;
		return DateTime.FromOADate(val).ToShortDateString();
	}
	public string getLabelYFor(double frac)
	{
		double val = _ymin + (_ymax - _ymin) * frac;
		return val.ToString("0.000");
	}
}

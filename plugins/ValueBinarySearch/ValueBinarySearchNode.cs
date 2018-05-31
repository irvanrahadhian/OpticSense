#region usings
using System;
using System.ComponentModel.Composition;

using VVVV.PluginInterfaces.V1;
using VVVV.PluginInterfaces.V2;
using VVVV.Utils.VColor;
using VVVV.Utils.VMath;

using VVVV.Core.Logging;
#endregion usings

namespace VVVV.Nodes
{
	#region PluginInfo
	[PluginInfo(Name = "BinarySearch", Category = "Value", Help = "Basic template with one value in/out", Tags = "c#")]
	#endregion PluginInfo
	public class ValueBinarySearchNode : IPluginEvaluate
	{
		#region fields & pins
		[Input("InputWhite", DefaultValue = 0, IsBang=true)]
		public IDiffSpread<bool> FInputWhite;
		
		[Input("InputBlack", DefaultValue = 0, IsBang=true)]
		public IDiffSpread<bool> FInputBlack;
		
		[Input("Reset", DefaultValue = 0, IsBang=true)]
		public IDiffSpread<bool> FInputReset;
		
		[Input("Count", DefaultValue = 1)]
		public ISpread<int> FInputCount;

		[Output("Output")]
		public ISpread<int> FOutput;
		
		[Output("OutputL")]
		public ISpread<int> FOutputL;
		
		[Output("OutputR")]
		public ISpread<int> FOutputR;
		
		[Output("IterationCount")]
		public ISpread<double> FOutputIterationCount;
		
		[Import()]
		public ILogger FLogger;
		#endregion fields & pins

		//called when data for any output pin is requested
		public void Evaluate(int SpreadMax)
		{
			FOutputL.SliceCount = 1;
			FOutputR.SliceCount = 1;
			FOutputIterationCount.SliceCount = 1;
			
			if(FOutputIterationCount[0] >= Math.Log(FInputCount[0], 2)+1) {
				FOutput.SliceCount = 1;
				if(FOutputL[0] == FOutputR[0]) {
					FOutput[0] = FOutputL[0];
				}
				else {
					FOutput[0] = FOutputR[0];
				}
			}
			else {
				if(FInputWhite.IsChanged) {
					if(FInputWhite[0]) {
						if(FOutputIterationCount[0] % 1 != 0) {
							int m = (FOutputR[0] - FOutputL[0] - 1) / 2;
							FOutputR[0] = FOutputL[0] + m;
						}
						FOutputIterationCount[0]+=0.5;
//						FOutputIterationCount[0]+=1;
					}
				}
				if(FInputBlack.IsChanged) {
//					if(FOutputIterationCount[0] != 0) {
						if(FInputBlack[0]) {
							if(FOutputIterationCount[0] % 1 != 0) {
								int m = (FOutputR[0] - FOutputL[0] - 1) / 2;
								FOutputL[0] = FOutputR[0] + 1;
								FOutputR[0] = FOutputL[0] + m;
							}
							FOutputIterationCount[0]+=0.5;
//							FOutputIterationCount[0]+=1;
						}
//					}
				}
			}
			
			if(FInputReset.IsChanged || FInputReset[0]) {
				FOutput.SliceCount = 0;
				FOutputL[0] = 0;
				FOutputR[0] = FInputCount[0] / 2 - 1;
				FOutputIterationCount[0] = 0;
			}
		}
	}
}

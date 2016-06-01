using System;

public class TCombinator
{
                                  
    int CP;
                     
    //---------------------------------------   Set/get n Elements
    int var_Element;
           
    public int n_Element
    {
	    get
	    {   return this.var_Element;   }
	    set
	    {   
            this.var_Element = value; 
            Array.Resize(ref OrgSet, value+1);
            Initializ_Element();
        }
    }
    //---------------------------------------   Set/get k Combin
    int var_Combin;
           
    public int k_Combin
    {
	    get
	    {   return this.var_Combin;   }
	    set
	    {   
            this.var_Combin = value;  
            Array.Resize(ref CombSet, value+1);
            Initializ_Combin();
        }
    }
    //---------------------------------------     //ini both arrays sizes with at least one element then resize with any sizes needed
    public int[] OrgSet = new int[1];            //array holds the elements that will be combined
    public int[] CombSet = new int[1];            //array of series of +ve integers first element = 1
    //---------------------------------------            
    int[] Max  = new int[32];                     //array of +ve series, counts descending from (element size) to (0) 
    public bool Finished = false;
    //================================================================================================================
    public  TCombinator(int elementSize,int CombinationSize)
    {
        n_Element = elementSize;
        k_Combin = CombinationSize; 
    }
    //================================================================== this Ini func can be used to reset the Arrays and to start the combine from the begining
    public void Initializ_Element()
    {
        //-------------------------------------------------------------------  //ini Element array with series of +ve integers up to its length
        for (int x=0;       x < OrgSet.Length;       x++)   
                    
            OrgSet[x]  = x;

        //-------------------------------------------------------------------  //ini CombAry with the elements
        CP = (var_Combin);            
        Finished = false;                       //Finish Flag, indicates to the End               
    }
    //================================================================== ini Max Ary against each combAry[] identical element
    public void Initializ_Combin()
    {
        for (int x = 1;     x <= var_Combin;            x++) 
                    
            CombSet[x] = x;                     
            
        //-------------------------------------------------------------------  
        int v= CombSet.Length-2;

        for (int x = 1;     x <= CombSet.Length-1;      x++)
        {
            Max[x] = (OrgSet[var_Element - v]);
            v--;
        }
        //-------------------------------------------------------------------
        CP = (var_Combin);            
        Finished = false;                       //Finish Flag, indicates to the End           
    }
    //================================================================== NextCombin() with each call generates new combination updated in CombSet[]
    public void NextCombin()
    {
        if (Finished) return;                                           //End of process
                
        if (CombSet[1] >= Max[1]) { Finished = true;     return; }      //last location (which is first element)

        //----------------------------------------------------------------------------
        if (CombSet[CP] == Max[CP])
        {
            //------------------------------------------------------------------------
            //Loop & step down TC till find CombAry[] != Max[]
            //------------------------------------------------------------------------
            while (CombSet[CP] == Max[CP])
            {
                CP--;
            }
            //-------------------------------------------------------------------------
            //in case the element < its Max value Advance element at 
            //[TC] one step then re-adjust all next elements from location (TC) upto 
            //last location (CombArySZ),,  Or Advance element at [TC] one Step directly
            //-------------------------------------------------------------------------
            int Loc = CombSet[CP];

            if (CombSet[CP]+1 == Max[CP])
                                          
                CombSet[CP] = ++Loc;
                        
            else
                for (int t = CP;        t <= var_Combin;      t++)
                {
                    CombSet[t] = ++Loc;                                                                                  
                }                        
            //-------------------------------------------------------------------------
            //after this element readjustment, alwayes TC must be reset to 
            //the last element in CombAry[]
            //-------------------------------------------------------------------------
            CP = var_Combin;                      
                   
            return;
        }

        else
                    
            CombSet[CP]++;  
             
        // the new combination result has been generated, that you can display or store.
    }
    //================================================================================================================
    public UInt64 CalcCombNum(UInt64 n,UInt64 r)
    {
        UInt64 val;
        UInt64 fn = Factorial(n);
        UInt64 fr = Factorial(r);
        UInt64 fn_r= Factorial(n-r);
        return val =  ( fn / (fr * fn_r) );        //nCr = n!/(r! * (n-r)!)
    }
    //===============================================================================================================
    public UInt64 Factorial(UInt64 n)
    {
        UInt64 val=1;
        while (n > 0)
        {
            val *= n;
            n--;
        }
        return val;
    }
            
}


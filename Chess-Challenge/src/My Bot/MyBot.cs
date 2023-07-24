using ChessChallenge.API;
using System.Linq;
using System.Numerics;
using System;
using System.Collections.Generic;

public class MyBot : IChessBot
{
    bool weAreWhite = true;
    List<Move> mList = new List<Move>();
    public Move Think(Board board, Timer timer)
    {
        weAreWhite = board.IsWhiteToMove;
        mList = new List<Move>();
        var t = MinMax(board,4,int.MinValue,int.MaxValue,true);
        Console.WriteLine(t.Item2);
        return t.Item1!.Value;
    }

    public (Move?,float) MinMax(Board b,int depth,double alpha, double beta,bool maximizingPlayer){
        if(depth==0){
            return (null,this.eval(b));
        }
        if (maximizingPlayer){
            float? maxEval = null;
            Move? maxMove = null;
            foreach(var child in b.GetLegalMoves().OrderBy(m=>m.IsCapture ? 0 : 1)){
                mList.Add(child);
                b.MakeMove(child);
                var eval = MinMax(b,depth-1,alpha,beta,false).Item2;
                b.UndoMove(child);
                mList.RemoveAt(mList.Count-1);
                if(maxEval==null||eval>maxEval){
                    maxMove = child;
                }
                maxEval = Math.Max(maxEval??float.MinValue,eval);
                alpha = Math.Max(alpha,eval);
                if(beta <= alpha){
                    break;
                }
            }
            if(maxMove==null&&depth==5){
                Console.WriteLine("what");
            }
            return (maxMove,maxEval??(b.IsDraw()?0:int.MinValue));
        }else{
            float? minEval = null;
            Move? minMove = null;
            foreach(var child in b.GetLegalMoves().OrderBy(m=>m.IsCapture ? 0 : 1)){
                mList.Add(child);
                b.MakeMove(child);
                var eval = MinMax(b,depth-1,alpha,beta,true).Item2;
                b.UndoMove(child);
                mList.RemoveAt(mList.Count-1);
                if(minEval==null||eval<minEval){
                    minMove = child;
                }
                minEval = Math.Min(minEval??float.MaxValue,eval);
                beta = Math.Min(beta,eval);
                if(beta <= alpha){
                    break;
                }
            }
            return (minMove,minEval??(b.IsDraw()?0:int.MaxValue));
        }
    }

    public float eval(Board b){ //change to isWhiteToMove?
        if(b.IsInCheckmate()){
            return 10000 * (b.IsWhiteToMove!=weAreWhite?1:-1);
        }
        var value = 0f;
        var isEnemy = false;
        var extra = 0f;
        var extra2 = 0f;
        var enemyKingSquare = b.GetKingSquare(!weAreWhite);
        foreach (var pL in b.GetAllPieceLists()){
            isEnemy = pL.IsWhitePieceList != weAreWhite;
            
            foreach(var p in pL){
                float factor = 1;
                if(!isEnemy){
                    if (b.SquareIsAttackedByOpponent(p.Square)){
                        factor*=0.9f;
                    }
                    if(!p.IsPawn){
                    var distance = MathF.Abs(p.Square.File - enemyKingSquare.File)+MathF.Abs(p.Square.Rank - enemyKingSquare.Rank);
                    extra += (15-distance)*1/30f;
                    }else{
                        extra2 += (8-MathF.Abs(p.Square.Rank-(weAreWhite?8:0)))*1/20f;
                    }
                }
                if(isEnemy){
                    if (b.SquareIsAttackedByOpponent(p.Square)){
                        factor*=1.05f;
                    }
                }
                value += factor * ToValue(p.PieceType)*( isEnemy?-1f:1f);
            }
        }
        if(b.IsDraw()){
            return 0;
        }
        value+=extra;
        value +=extra2;
        return value;
    }



    public static float ToValue(PieceType p) => p switch{
        PieceType.None => throw new ArgumentOutOfRangeException("Not right"),
        PieceType.Pawn => 1,
        PieceType.Bishop => 3,
        PieceType.Knight => 3,
        PieceType.Rook => 5,
        PieceType.Queen => 9,
        PieceType.King => 9,
    };
}
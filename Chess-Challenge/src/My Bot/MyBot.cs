using ChessChallenge.API;
using System.Linq;
using System.Numerics;
using System;

public class MyBot : IChessBot
{
    public Move Think(Board board, Timer timer)
    {
        return MinMax(board,5,int.MinValue,int.MaxValue,true).Item1!.Value;
    }

    public (Move?,int) MinMax(Board b,int depth,double alpha, double beta,bool maximizingPlayer){
        if(depth==0){
            return (null,this.eval(b));
        }
        if (maximizingPlayer){
            var maxEval = int.MinValue;
            Move? maxMove = null;
            foreach(var child in b.GetLegalMoves().OrderBy(m=>m.IsCapture ? 0 : 1)){
                b.MakeMove(child);
                var eval = MinMax(b,depth-1,alpha,beta,false).Item2;
                b.UndoMove(child);
                if(eval>maxEval){
                    maxMove = child;
                }
                maxEval = Math.Max(maxEval,eval);
                alpha = Math.Max(alpha,eval);
                if(beta <= alpha){
                    break;
                }
            }
            return (maxMove,maxEval);
        }else{
            var minEval = int.MaxValue;
            Move? minMove = null;
            foreach(var child in b.GetLegalMoves().OrderBy(m=>m.IsCapture ? 0 : 1)){
                b.MakeMove(child);
                var eval = MinMax(b,depth-1,alpha,beta,true).Item2;
                b.UndoMove(child);
                if(eval<minEval){
                    minMove = child;
                }
                minEval = Math.Min(minEval,eval);
                beta = Math.Min(alpha,eval);
                if(beta <= alpha){
                    break;
                }
            }
            return (minMove,minEval);
        }
    }

    public int eval(Board b){
        return b.GetPieceList(PieceType.Pawn,!b.IsWhiteToMove).Count
        + b.GetPieceList(PieceType.Bishop,!b.IsWhiteToMove).Count*3
        + b.GetPieceList(PieceType.Knight,!b.IsWhiteToMove).Count*3
        + b.GetPieceList(PieceType.Rook,!b.IsWhiteToMove).Count*5
        + b.GetPieceList(PieceType.Queen,!b.IsWhiteToMove).Count*8
        -b.GetPieceList(PieceType.Pawn,b.IsWhiteToMove).Count
        - b.GetPieceList(PieceType.Bishop,b.IsWhiteToMove).Count*3
        - b.GetPieceList(PieceType.Knight,b.IsWhiteToMove).Count*3
        - b.GetPieceList(PieceType.Rook,b.IsWhiteToMove).Count*5
        - b.GetPieceList(PieceType.Queen,b.IsWhiteToMove).Count*8;
    }
}
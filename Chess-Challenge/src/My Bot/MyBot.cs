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
        Move? move = MinMax(board,4,int.MinValue,int.MaxValue,true).Item1;
        return move.Value;
    }

    public (Move?,int) MinMax(Board b,int depth,double alpha, double beta,bool maximizingPlayer){
        if(depth==0){
            return (null,this.eval(b));
        }
        if (maximizingPlayer){
            int? maxEval = null;
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
                maxEval = Math.Max(maxEval??int.MinValue,eval);
                alpha = Math.Max(alpha,eval);
                if(beta <= alpha){
                    break;
                }
            }
            if(maxMove==null&&depth==5){
                Console.WriteLine("what");
            }
            return (maxMove,maxEval??int.MinValue);
        }else{
            int? minEval = null;
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
                minEval = Math.Min(minEval??int.MaxValue,eval);
                beta = Math.Min(beta,eval);
                if(beta <= alpha){
                    break;
                }
            }
            return (minMove,minEval??int.MaxValue);
        }
    }

    public int eval(Board b){ //change to isWhiteToMove?
        if(b.IsInCheckmate()){
            return 10000 * (b.IsWhiteToMove!=weAreWhite?1:-1);
        }
        return b.GetPieceList(PieceType.Pawn,weAreWhite).Count
        + b.GetPieceList(PieceType.Bishop,weAreWhite).Count*3
        + b.GetPieceList(PieceType.Knight,weAreWhite).Count*3
        + b.GetPieceList(PieceType.Rook,weAreWhite).Count*5
        + b.GetPieceList(PieceType.Queen,weAreWhite).Count*8
        -b.GetPieceList(PieceType.Pawn,!weAreWhite).Count
        - b.GetPieceList(PieceType.Bishop,!weAreWhite).Count*3
        - b.GetPieceList(PieceType.Knight,!weAreWhite).Count*3
        - b.GetPieceList(PieceType.Rook,!weAreWhite).Count*5
        - b.GetPieceList(PieceType.Queen,!weAreWhite).Count*8;
    }
}
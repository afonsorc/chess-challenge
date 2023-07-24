using ChessChallenge.API;
using System;
using System.Linq;
using System.Numerics;
using System.Collections.Generic;
using System.IO;


public class MyBot : IChessBot{

    const bool DEBUG = false;

    // Last value for initialization of moves
    int[] pieceValue = { 100, 325, 350, 500, 900, 10000, 10001 };

    public Move Think(Board board, Timer timer){
        
        int depth = 4;
        Move bestMove = getBestMove(board, depth);

        //if (DEBUG) Console.WriteLine((board.PlyCount + 1) / 2);
        return bestMove;
    }


    public int evaluate(Board board){

        int evaluation = 0;

        for(int p = 0; p < 5; p++){
            evaluation += pieceValue[p] * board.GetAllPieceLists()[p].Count;
            evaluation -= pieceValue[p] * board.GetAllPieceLists()[p + 6].Count;
        }

        return board.IsWhiteToMove ? evaluation : -evaluation;
    }


    public int alphaBeta(Board board, int depth, int alpha, int beta){

        if (depth == 0) return evaluate(board);

        int max = -pieceValue[6];

        Move[] moves = moveOrdering(board.GetLegalMoves());
        foreach (Move move in moves){

            board.MakeMove(move);
            int evaluation = -alphaBeta(board, depth - 1, -beta, -alpha);
            board.UndoMove(move);

            // upper bound, opponent will avoid our move
            if(evaluation >= beta) return beta;

            // lower bound, looking to cut worse moves
            if(evaluation > alpha) alpha = evaluation;
        }
        return alpha;
    }
    

    public Move getBestMove(Board board, int depth){

        int max = -pieceValue[6];
        Move bestMove = Move.NullMove;

        Move[] moves = moveOrdering(board.GetLegalMoves());
        foreach (Move move in moves){

            board.MakeMove(move);
            int evaluation = -alphaBeta(board, depth - 1, -pieceValue[5], pieceValue[5]);
            board.UndoMove(move);

            if (evaluation > max){
                max = evaluation;
                bestMove = move;
            }
            if (DEBUG) Console.WriteLine(move + " " + evaluation);
        }
        if(DEBUG) Console.WriteLine("Best" + bestMove + " " + max);
        return bestMove;
    }


    public Move[] moveOrdering(Move[] moves){

        int[] moveScore = new int[moves.Length];

        for(int i = 0; i < moves.Length; i++){
            
            if(moves[i].IsPromotion && moves[i].IsCapture) moveScore[i] = 10;
            else if(moves[i].IsPromotion) moveScore[i] = 9;
            else if(moves[i].IsCapture && (int)moves[i].MovePieceType < (int)moves[i].CapturePieceType) moveScore[i] = 8;
            else if(moves[i].IsCapture && (int)moves[i].MovePieceType == (int)moves[i].CapturePieceType) moveScore[i] = 7;
            else if(moves[i].IsCapture && (int)moves[i].MovePieceType > (int)moves[i].CapturePieceType) moveScore[i] = 6;
            else if(moves[i].MovePieceType == PieceType.Queen) moveScore[i] = 5;
            else if(moves[i].MovePieceType == PieceType.Rook) moveScore[i] = 4;
            else if(moves[i].MovePieceType == PieceType.Bishop || moves[i].MovePieceType == PieceType.Knight) moveScore[i] = 3;
            else if(moves[i].MovePieceType == PieceType.Pawn) moveScore[i] = 2;
            else moveScore[i] = 1;
        }

        sortMoveList(moves, moveScore);
        return moves;
    }

    public void sortMoveList(Move[] moves, int[] score){
        
        //System.Console.WriteLine("UNSORTED");
        //for(int i = 0; i < moves.Length; i++) System.Console.WriteLine(moves[i] + " " + score[i]);

        for(int i = 0; i < moves.Length; i++){
            int biggestValue = i;
            for(int j = i + 1; j < moves.Length; j++){
                if(score[j] > score[biggestValue]) biggestValue = j;
            }
            if(biggestValue != i){
                //System.Console.WriteLine("SWITCH " + moves[i] + " and " + moves[biggestValue]);
                Move temp = moves[biggestValue];
                moves[biggestValue] = moves[i];
                moves[i] = temp;
                int tempint = score[biggestValue];
                score[biggestValue] = score[i];
                score[i] = tempint;
            }
        }

        //System.Console.WriteLine("SORTED");
        //for(int i = 0; i < moves.Length; i++) System.Console.WriteLine(moves[i] + " " + score[i]);
        return;
    }
}
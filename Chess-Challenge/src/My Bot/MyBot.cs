using ChessChallenge.API;
using System;
using System.Linq;
using System.Numerics;
using System.Collections.Generic;
using System.IO;


public class MyBot : IChessBot{

    const bool DEBUG = true;

    int[] pieceValue = { 100, 325, 350, 500, 900};
    int checkmateValue = 10000;

    public Move Think(Board board, Timer timer){

        int depth = 3;
        Move bestMove = getBestMove(board, depth);
        if (DEBUG) Console.WriteLine((board.PlyCount + 1) / 2);
        return bestMove;
    }


    public int evaluate(Board board){

        int evaluation = 0;
        for(int pl = 0; pl < 5; pl++){
            evaluation += pieceValue[pl] * board.GetAllPieceLists()[pl].Count;
            evaluation -= pieceValue[pl] * board.GetAllPieceLists()[pl + 6].Count;
        }
        return board.IsWhiteToMove ? evaluation : -evaluation;
    }


    public int search(Board board, int depth){

        if (depth == 0) return evaluate(board);
        if (board.IsInCheckmate()) return -checkmateValue;

        int max = -checkmateValue - 1;

        foreach(Move move in board.GetLegalMoves()){

            board.MakeMove(move);
            int evaluation = -search(board, depth - 1);
            board.UndoMove(move);

            if (evaluation > max) max = evaluation;
        }
        return max;
    }
    

    public Move getBestMove(Board board, int depth){

        int max = -checkmateValue - 1;
        Move bestMove = Move.NullMove;

        foreach (Move move in board.GetLegalMoves()){

            board.MakeMove(move);
            int evaluation = -search(board, depth - 1);
            board.UndoMove(move);

            if (evaluation > max){
                max = evaluation;
                bestMove = move;
            }
            if (DEBUG) Console.WriteLine(move + " " + (evaluation/100.0f).ToString("F"));
        }
        if(DEBUG) Console.WriteLine("Best" + bestMove + " " + (max/100.0f).ToString("F"));
        return bestMove;
    }
}
using ChessChallenge.API;
using System;
using System.Linq;
using System.Numerics;
using System.Collections.Generic;
using System.IO;


public class MyBot : IChessBot{

    float[] pieceValue = { 1.0f, 3.2f, 3.4f, 5.0f, 9.5f};

    public Move Think(Board board, Timer timer)
    {
        int depth = 4;
        Console.WriteLine((board.PlyCount + 1) / 2);
        Move bestMove = getBestMove(board, depth);
        Console.WriteLine("Best Move: " + bestMove);
        return bestMove;
    }


    public float evaluate(Board board){

        float evaluation = 0.0f;
        int whoToMove = 1;

        if (!board.IsWhiteToMove) whoToMove = -1;
        for(int pl = 0; pl < 5; pl++){
            evaluation += pieceValue[pl] * board.GetAllPieceLists()[pl].Count;
            evaluation -= pieceValue[pl] * board.GetAllPieceLists()[pl + 6].Count;
        }
        return (float)(evaluation * whoToMove);
    }


    public float search(Board board, int depth){

        if (depth == 0) return evaluate(board);

        if (board.IsInCheckmate()) return -1000.0f;

        float max = -1000.0f;
        Move[] moves = board.GetLegalMoves();
        foreach(Move move in moves){
            board.MakeMove(move);
            float evaluation = -search(board, depth - 1);
            board.UndoMove(move);
            if (evaluation > max){
                max = evaluation;
            }
        }
        return max;
    }
    

    public Move getBestMove(Board board, int depth){

        Move[] moves = board.GetLegalMoves();
        Move bestMove = moves[0];
        float max = -1000.0f;

        foreach (Move move in moves){

            board.MakeMove(move);
            float evaluation = -search(board, depth - 1);
            board.UndoMove(move);

            if (evaluation > max){
                max = evaluation;
                bestMove = move;
            }
            Console.WriteLine(move + " " + evaluation.ToString("F"));
        }
        Console.WriteLine("Best" + bestMove + " " + max.ToString("F"));
        return bestMove;
    }
}
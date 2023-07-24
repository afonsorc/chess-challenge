using ChessChallenge.API;
using System;

public class MyBot : IChessBot{

    // Last value for initialization of moves
    int[] pieceValue = { 100, 320, 325, 500, 975, 10000, 32767 };

    public Move Think(Board board, Timer timer){
        
        // depth should be even to end on opponent's turn 
        int depth = 5;
        Move bestMove = GetBestMove(board, depth);

        Console.WriteLine((board.PlyCount + 1) / 2);
        Console.WriteLine(bestMove);
        return bestMove;
    }


    public int Evaluate(Board board){

        int evaluation = 0;
        if(board.IsDraw()) return 0;
        for(var p = 0; p < 5; p++){
            evaluation += pieceValue[p] * (int)board.GetAllPieceLists()[p].Count;
            evaluation -= pieceValue[p] * (int)board.GetAllPieceLists()[p + 6].Count;
        }
        return board.IsWhiteToMove ? evaluation : -evaluation;
    }


    public int AlphaBetaSearch(Board board, int depth, int alpha, int beta){

        if (depth == 0) return QuiescenceSearch(board, alpha, beta);

        foreach(Move move in MoveOrdering(board.GetLegalMoves())){

            board.MakeMove(move);
            int evaluation = -AlphaBetaSearch(board, depth - 1, -beta, -alpha);
            board.UndoMove(move);

            // upper bound beta, opponent will avoid our move
            // lower bound alpha, looking to cut worse moves
            if(evaluation >= beta) return beta;
            if(evaluation > alpha) alpha = evaluation;
        }
        return alpha;
    }


    public int QuiescenceSearch(Board board, int alpha, int beta){
        
        int standPat = Evaluate(board);
        if(standPat >= beta) return beta;
        if(standPat > alpha) alpha = standPat;

        foreach(Move move in MoveOrdering(board.GetLegalMoves(true))){
                
            board.MakeMove(move);
            int evaluation = -QuiescenceSearch(board, -beta, -alpha);
            board.UndoMove(move);

            if(evaluation >= beta) return beta;
            if(evaluation > alpha) alpha = evaluation;
        }
        return alpha;
    }


    public Move[] MoveOrdering(Move[] moves){

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
        SortMoveList(moves, moveScore);
        return moves;
    }

    public void SortMoveList(Move[] moves, int[] score){
        
        //System.Console.WriteLine("UNSORTED");
        //for(int i = 0; i < moves.Length; i++) System.Console.WriteLine(moves[i] + " " + score[i]);
        for(int i = 0; i < moves.Length; i++){
            int biggestValue = i;
            for(int j = i + 1; j < moves.Length; j++)
                if(score[j] > score[biggestValue]) biggestValue = j;
            if(biggestValue != i){
                //System.Console.WriteLine("SWITCH " + moves[i] + " and " + moves[biggestValue]);
                (moves[i], moves[biggestValue]) = (moves[biggestValue], moves[i]);
                (score[i], score[biggestValue]) = (score[biggestValue], score[i]);
            }
        }
        //System.Console.WriteLine("SORTED");
        //for(int i = 0; i < moves.Length; i++) System.Console.WriteLine(moves[i] + " " + score[i]);
        return;
    }


    public Move GetBestMove(Board board, int depth){

        int max = -pieceValue[6];
        Move bestMove = Move.NullMove;

        foreach (Move move in MoveOrdering(board.GetLegalMoves())){

            board.MakeMove(move);
            int evaluation = -AlphaBetaSearch(board, depth - 1, -pieceValue[5], pieceValue[5]);
            board.UndoMove(move);

            if (evaluation > max){
                max = evaluation;
                bestMove = move;
            }
           //Console.WriteLine(move + " " + evaluation);
        }
        return bestMove;
    }
}
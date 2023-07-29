# Chess Coding Challenge (C#)
Welcome to the [chess coding challenge](https://youtu.be/iScy18pVR58)! This is a friendly competition in which your goal is to create a small chess bot (in C#) using the framework provided in this repository.
Once submissions close, these bots will battle it out to discover which bot is best!

I will then create a video exploring the implementations of the best and most unique/interesting bots.
I also plan to make a small game that features these most interesting/challenging entries, so that everyone can try playing against them.


## To Implement:
- [x] Material evalutation
- [x] Negamax search
- [x] Alpha beta search
- [x] Move ordering and sorting
- [x] Transposition Tables with Zobrist Hashing
- [x] Quiescence search by the end of alphabeta
- [x] Repetition/Stalemate/Insufficient Material check
- [x] Basic PST (Simple Evaluation)
- [ ] Dynamic Evaluation (Pesto PST, mobility, king safety)
- [ ] Aspiration Windows


## Version History

### 1.0.0
* Implemented basic depth 1 evaluation function with standard piece weights
* Prioritize specific moves in the below order
  * Always go for M1 moves
  * Capture the most valuable piece available
  * Check the opposite King
  * Else random move

### 2.0.0
* Implemented NegaMax with custom depth search
  * Able to run 1000 games at decent speed with depth 4 

### 2.1.0
* Changed evaluation from floats to ints

### 3.0.0
* Implemented alpha-beta with ordered moves
  * Simple move scoring criteria, based on promotions/captures/moved piece
  * Selection sort on the move list
  * Able to run 1000 games at decent speed with depth 5

### 3.1.0
* Added quiescence search after alpha-beta to finish capture chain calculations
* Adjusted piece weights

### 3.2.0
* Implemented semi Pawn and Knight Square Tables

### 4.0.0
* Implemented transposition tables from Zobrist hashing
* Implemented iterative deepening search
* Added cuttofs in search for all draw conditions
* Running tests at max depth 4, depth 5 is too slow and often loses by timeout

## Version Comparison vs EvilBot

### 1.0.0 vs Control (+359 =640 -1)
&#x1F40A;&#x1F40A;&#x1F40A;&#x1F40A;&#x1F40A;&#x1F40A;&#x1F40A;&#x1F40A;&#x1F40A;&#x1F40A;&#x1F40A;&#x1F40A;&#x1F40A;
&#x1F42A;&#x1F42A;&#x1F42A;&#x1F42A;&#x1F42A;&#x1F42A;&#x1F42A;&#x1F42A;&#x1F42A;&#x1F42A;&#x1F42A;&#x1F42A;&#x1F42A;&#x1F42A;&#x1F42A;&#x1F42A;&#x1F42A;&#x1F42A;&#x1F42A;&#x1F42A;&#x1F42A;&#x1F42A;&#x1F42A;
&#x1F426;<br>

### 2.0.0 vs Control (+208 =792 -0)
&#x1F40A;&#x1F40A;&#x1F40A;&#x1F40A;&#x1F40A;&#x1F40A;&#x1F40A;&#x1F40A;
&#x1F42A;&#x1F42A;&#x1F42A;&#x1F42A;&#x1F42A;&#x1F42A;&#x1F42A;&#x1F42A;&#x1F42A;&#x1F42A;&#x1F42A;&#x1F42A;&#x1F42A;&#x1F42A;&#x1F42A;&#x1F42A;&#x1F42A;&#x1F42A;&#x1F42A;&#x1F42A;&#x1F42A;&#x1F42A;&#x1F42A;&#x1F42A;&#x1F42A;&#x1F42A;&#x1F42A;&#x1F42A;&#x1F42A;<br>

### 3.0.0 vs Control (+557 =443 -0)
&#x1F40A;&#x1F40A;&#x1F40A;&#x1F40A;&#x1F40A;&#x1F40A;&#x1F40A;&#x1F40A;&#x1F40A;&#x1F40A;&#x1F40A;&#x1F40A;&#x1F40A;&#x1F40A;&#x1F40A;&#x1F40A;&#x1F40A;&#x1F40A;&#x1F40A;&#x1F40A;&#x1F40A;
&#x1F42A;&#x1F42A;&#x1F42A;&#x1F42A;&#x1F42A;&#x1F42A;&#x1F42A;&#x1F42A;&#x1F42A;&#x1F42A;&#x1F42A;&#x1F42A;&#x1F42A;&#x1F42A;&#x1F42A;&#x1F42A;<br>


### 4.0.0 vs EvilBot (+999 =0 -1)
&#x1F40A;&#x1F40A;&#x1F40A;&#x1F40A;&#x1F40A;&#x1F40A;&#x1F40A;&#x1F40A;&#x1F40A;&#x1F40A;&#x1F40A;&#x1F40A;&#x1F40A;&#x1F40A;&#x1F40A;&#x1F40A;&#x1F40A;&#x1F40A;&#x1F40A;&#x1F40A;&#x1F40A;&#x1F40A;&#x1F40A;&#x1F40A;&#x1F40A;&#x1F40A;&#x1F40A;&#x1F40A;&#x1F40A;&#x1F40A;&#x1F40A;&#x1F40A;&#x1F40A;&#x1F40A;&#x1F40A;&#x1F40A;
&#x1F426;<br>

## Version Progress Comparison
### 2.0.0 vs 1.0.0 (+291 =709 -0)
&#x1F40A;&#x1F40A;&#x1F40A;&#x1F40A;&#x1F40A;&#x1F40A;&#x1F40A;&#x1F40A;&#x1F40A;&#x1F40A;&#x1F40A;
&#x1F42A;&#x1F42A;&#x1F42A;&#x1F42A;&#x1F42A;&#x1F42A;&#x1F42A;&#x1F42A;&#x1F42A;&#x1F42A;&#x1F42A;&#x1F42A;&#x1F42A;&#x1F42A;&#x1F42A;&#x1F42A;&#x1F42A;&#x1F42A;&#x1F42A;&#x1F42A;&#x1F42A;&#x1F42A;&#x1F42A;&#x1F42A;&#x1F42A;&#x1F42A;<br>

### 3.0.0 vs 2.0.0 ()

### 4.0.0 vs 3.0.0 ()
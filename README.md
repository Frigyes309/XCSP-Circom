# XCSP to Circom Circuit converter

A program to transform your XCSP code into a Circom cirtuit, therefore you can check if thw result of your constraint system written in XCSP is really a valid solution. You will get a Zero-Knowledge Proof of the result.

## Usage

 1. Clone the project
 2. Put the constraint.xcsp and solution.json files in the input folder
 3. In the root folder use the proper .bacth file
    - run.bat: If u want to run with log about the running
    - runCSharpMeasure.bat: If u only want to run and measure the C# code
    - runWithTime.bat: Run the whole process without logs and with a timer. The summery is in the end
 4. The output can be seen in the output folder. Detailed "output" can be seen in the public.json file

## Author

Smuk Ferenc

## Acknowledgements

This work created under my Bachelor’s Thesis with the title of Distributed ledger support of bond issuance in 2024.

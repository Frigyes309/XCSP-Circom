Overall Circom code and proof generation process

```
Delete old files that might ruin the process
Call the C# code on the input folder
Compile the generated circom code
Generate witness
Setup snarkjs
Generate .zkey files
Generate verification key
Generate proof
Verify proof
```

C# PseudoCode at step 2

```
Begin Main
    Read xcsp_code
    Read solution_for_constraints
    Create output file
    Add header to output file (version and imports)

    Filter each unique constraint from xcsp_code
    For each unique_constraint_type
        Generate constraint function
        For each constraint_component
            If constraint_component is not created yet
                Generate constant function
            End If
        End For
        Write created code to output file
    End For

    Add main component to output file
    For each constraint_component in input
        Generate call to constraint function with proper parameters
    End For

    Set main component as default component in output file

    Write out output file
    Close output file
End Main
```

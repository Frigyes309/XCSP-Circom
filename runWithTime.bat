@echo off
echo Starting the process...
echo Cleaning up old files that might ruin the process...
call cd output
for /f "delims=" %%a in ('.\timecmd cleanup.bat') do set cleanup_time=%%a
call cd ..
echo Cleaning up done...

echo Creating the Circom file...
for /f "delims=" %%a in ('.\timecmd dotnet run') do set dotnet_build_time=%%a
call cd output

echo Generating new files...
for /f "delims=" %%a in ('.\timecmd circom Converter.circom --r1cs --wasm --sym --c') do set circom_generate_time=%%a
for /f "delims=" %%a in ('.\timecmd node Converter_js/generate_witness.js Converter_js/Converter.wasm input.json witness.wtns') do set witness_generate_time=%%a
echo New files are ready...

echo Setting up snarkjs...
for /f "delims=" %%a in ('.\timecmd snarkjs powersoftau new bn128 12 pot12_0000.ptau -v') do set snarkjs_init_time=%%a
for /f "delims=" %%a in ('.\timecmd zkey1.bat') do set ptau_file_creation_time=%%a
for /f "delims=" %%a in ('.\timecmd snarkjs powersoftau prepare phase2 pot12_0001.ptau pot12_final.ptau -v') do set snarkjs_prepare_time=%%a
for /f "delims=" %%a in ('.\timecmd snarkjs groth16 setup Converter.r1cs pot12_final.ptau Converter_0000.zkey') do set groth16_setup_time=%%a
echo Zkey files are ready...

echo Generating proof...
for /f "delims=" %%a in ('.\timecmd zkey2.bat') do set zkey_generation_time=%%a
for /f "delims=" %%a in ('.\timecmd snarkjs zkey export verificationkey Converter_0001.zkey verification_key.json') do set verification_key_export_time=%%a
echo Verification key is ready...

for /f "delims=" %%a in ('.\timecmd snarkjs groth16 prove Converter_0001.zkey witness.wtns proof.json public.json') do set proof_generation_time=%%a
echo Verifying proof...
for /f "delims=" %%a in ('.\timecmd snarkjs groth16 verify verification_key.json public.json proof.json') do set proof_verification_time=%%a
echo Done...
echo.
echo ======= Times =======
echo Cleanup Time: %cleanup_time%
echo Dotnet Build Time: %dotnet_build_time%
echo Circom Generation Time: %circom_generate_time%
echo Witness Generation Time: %witness_generate_time%
echo Snarkjs Init Time: %snarkjs_init_time%
echo Ptau File Creation Time: %ptau_file_creation_time%
echo Snarkjs Preparation Time: %snarkjs_prepare_time%
echo Groth16 Setup Time: %groth16_setup_time%
echo Zkey Generation Time: %zkey_generation_time%
echo Verification Key Export Time: %verification_key_export_time%
echo Proof Generation Time: %proof_generation_time%
echo Proof Verification Time: %proof_verification_time%
echo ====================
echo.

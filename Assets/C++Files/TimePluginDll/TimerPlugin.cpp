#include<chrono>

using namespace std::chrono;

extern "C"{

	static high_resolution_clock::time_point g_start;

	__declspec (dllexport) void startTimer() {
		g_start = high_resolution_clock::now();
	}

	__declspec (dllexport) double getElapsedTime() {
		auto end = high_resolution_clock::now();
		duration<double> elapsed = end - g_start;
		return elapsed.count();
	}

	__declspec (dllexport) char getInputType() {
		auto end = high_resolution_clock::now();
		duration<double> elapsed = end - g_start;
		if (elapsed.count() < 0.1) {
			return '.';
		}
		else {
			return 'คั';
		}
	}
}
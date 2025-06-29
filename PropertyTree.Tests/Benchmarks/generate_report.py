#!/usr/bin/env python3
"""
Script to convert benchmark results to HTML report
"""

import json
import os
import sys
from datetime import datetime

def load_benchmark_results(json_file):
    """Load benchmark results from JSON file"""
    try:
        with open(json_file, 'r', encoding='utf-8') as f:
            return json.load(f)
    except FileNotFoundError:
        print(f"Error: {json_file} not found")
        return None
    except json.JSONDecodeError as e:
        print(f"Error: Failed to parse JSON file: {e}")
        return None

def format_time(ns):
    """Convert nanoseconds to readable format"""
    if ns < 1000:
        return f"{ns:.2f} ns"
    elif ns < 1000000:
        return f"{ns/1000:.2f} μs"
    elif ns < 1000000000:
        return f"{ns/1000000:.2f} ms"
    else:
        return f"{ns/1000000000:.2f} s"

def format_memory(bytes_val):
    """Convert bytes to readable format"""
    if bytes_val < 1024:
        return f"{bytes_val} B"
    elif bytes_val < 1024 * 1024:
        return f"{bytes_val/1024:.2f} KB"
    else:
        return f"{bytes_val/(1024*1024):.2f} MB"

def generate_html_report(results, output_file):
    """Generate HTML report"""
    
    # Calculate maximum values (for graph scaling)
    max_mean = max(result['mean'] for result in results) if results else 1
    max_memory = max(result['allocatedMemory'] for result in results) if results else 1
    
    html_content = f"""
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>PropWrapSharp Benchmark Results</title>
    <style>
        body {{
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
            margin: 0;
            padding: 20px;
            background-color: #f5f5f5;
        }}
        .container {{
            max-width: 1200px;
            margin: 0 auto;
            background-color: white;
            border-radius: 8px;
            box-shadow: 0 2px 10px rgba(0,0,0,0.1);
            overflow: hidden;
        }}
        .header {{
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            color: white;
            padding: 30px;
            text-align: center;
        }}
        .header h1 {{
            margin: 0;
            font-size: 2.5em;
            font-weight: 300;
        }}
        .header p {{
            margin: 10px 0 0 0;
            opacity: 0.9;
            font-size: 1.1em;
        }}
        .content {{
            padding: 30px;
        }}
        .benchmark-table {{
            width: 100%;
            border-collapse: collapse;
            margin-top: 20px;
            background-color: white;
            border-radius: 8px;
            overflow: hidden;
            box-shadow: 0 2px 10px rgba(0,0,0,0.1);
        }}
        .benchmark-table th {{
            background-color: #f8f9fa;
            padding: 15px;
            text-align: left;
            font-weight: 600;
            color: #495057;
            border-bottom: 2px solid #dee2e6;
        }}
        .benchmark-table td {{
            padding: 15px;
            border-bottom: 1px solid #dee2e6;
            vertical-align: top;
        }}
        .benchmark-table tr:hover {{
            background-color: #f8f9fa;
        }}
        .method-name {{
            font-weight: 600;
            color: #495057;
            font-family: 'Consolas', 'Monaco', monospace;
        }}
        .metric {{
            font-family: 'Consolas', 'Monaco', monospace;
            color: #6c757d;
        }}
        .bar-container {{
            width: 200px;
            height: 20px;
            background-color: #e9ecef;
            border-radius: 10px;
            overflow: hidden;
            position: relative;
        }}
        .bar {{
            height: 100%;
            background: linear-gradient(90deg, #007bff, #0056b3);
            border-radius: 10px;
            transition: width 0.3s ease;
        }}
        .bar-memory {{
            background: linear-gradient(90deg, #28a745, #1e7e34);
        }}
        .bar-label {{
            position: absolute;
            top: 50%;
            left: 50%;
            transform: translate(-50%, -50%);
            font-size: 12px;
            font-weight: 600;
            color: white;
            text-shadow: 1px 1px 2px rgba(0,0,0,0.5);
        }}
        .stats-grid {{
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
            gap: 15px;
            margin-top: 20px;
        }}
        .stat-card {{
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            color: white;
            padding: 20px;
            border-radius: 8px;
            text-align: center;
        }}
        .stat-value {{
            font-size: 2em;
            font-weight: 300;
            margin-bottom: 5px;
        }}
        .stat-label {{
            font-size: 0.9em;
            opacity: 0.9;
        }}
        .footer {{
            background-color: #f8f9fa;
            padding: 20px;
            text-align: center;
            color: #6c757d;
            border-top: 1px solid #dee2e6;
        }}
    </style>
</head>
<body>
    <div class="container">
        <div class="header">
            <h1>PropWrapSharp Benchmark Results</h1>
            <p>Generated: {datetime.now().strftime('%Y-%m-%d %H:%M:%S')}</p>
        </div>
        
        <div class="content">
            <div class="stats-grid">
                <div class="stat-card">
                    <div class="stat-value">{len(results)}</div>
                    <div class="stat-label">Benchmarks</div>
                </div>
                <div class="stat-card">
                    <div class="stat-value">{format_time(max_mean)}</div>
                    <div class="stat-label">Max Time</div>
                </div>
                <div class="stat-card">
                    <div class="stat-value">{format_memory(max_memory)}</div>
                    <div class="stat-label">Max Memory Usage</div>
                </div>
            </div>
            
            <table class="benchmark-table">
                <thead>
                    <tr>
                                        <th>Method Name</th>
                <th>Average Time</th>
                <th>Median</th>
                <th>Standard Deviation</th>
                <th>Min</th>
                <th>Max</th>
                <th>Ops/Sec</th>
                <th>Memory Usage</th>
                <th>Performance</th>
                    </tr>
                </thead>
                <tbody>
"""
    
    for result in results:
        method_name = result['method']
        mean_time = result['mean']
        median_time = result['median']
        std_dev = result['stdDev']
        min_time = result['min']
        max_time = result['max']
        ops_per_sec = result['operationsPerSecond']
        memory = result['allocatedMemory']
        
                    # Calculate performance bar width
        performance_width = (mean_time / max_mean) * 100 if max_mean > 0 else 0
        memory_width = (memory / max_memory) * 100 if max_memory > 0 else 0
        
        html_content += f"""
                    <tr>
                        <td class="method-name">{method_name}</td>
                        <td class="metric">{format_time(mean_time)}</td>
                        <td class="metric">{format_time(median_time)}</td>
                        <td class="metric">{format_time(std_dev)}</td>
                        <td class="metric">{format_time(min_time)}</td>
                        <td class="metric">{format_time(max_time)}</td>
                        <td class="metric">{ops_per_sec:.2f}</td>
                        <td class="metric">{format_memory(memory)}</td>
                        <td>
                            <div class="bar-container">
                                <div class="bar" style="width: {performance_width}%"></div>
                                <div class="bar-label">{performance_width:.1f}%</div>
                            </div>
                        </td>
                    </tr>
"""
    
    html_content += """
                </tbody>
            </table>
        </div>
        
        <div class="footer">
            <p>PropWrapSharp Benchmark Report | BenchmarkDotNet + Python HTML Generator</p>
        </div>
    </div>
</body>
</html>
"""
    
    with open(output_file, 'w', encoding='utf-8') as f:
        f.write(html_content)
    
    print(f"HTML report generated: {output_file}")

def main():
    """Main function"""
    json_file = "benchmark-results.json"
    output_file = "benchmark-report.html"
    
    # Check command line arguments
    if len(sys.argv) > 1:
        json_file = sys.argv[1]
    if len(sys.argv) > 2:
        output_file = sys.argv[2]
    
    print(f"Loading JSON file: {json_file}")
    results = load_benchmark_results(json_file)
    
    if results is None:
        print("Failed to load benchmark results")
        sys.exit(1)
    
    print(f"Loaded benchmark results: {len(results)} items")
    generate_html_report(results, output_file)

if __name__ == "__main__":
    main() 
﻿using System;

using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace Emission.Shading
{
    public class Shader : IDisposable
    {
        // public variables
        public string Name { get; private set; }
        public int Program {  get => _program;  }

        // private variables
        private int _program;
        private int _vertex;
        private int _fragment;

        // constructor
        public Shader(string path)
        {
            var file = ParseShader(path);
            Load(file.Item1, file.Item2);
        }
        
        // constructor
        public Shader(string vertex, string fragment)
        {
            Load(vertex, fragment);
        }

        /// <summary>
        /// Enable shader to be use.
        /// </summary>
        public void Start()
        {
            GL.UseProgram(_program);
        }

        /// <summary>
        /// Disable shader to be use.
        /// </summary>
        public void Stop()
        {
            GL.UseProgram(0);
        }

        /// <summary>
        /// Destroy Shader.
        /// </summary>
        public void Dispose()
        {
            GL.DeleteProgram(_program);
        }

        /// <summary>
        /// Return attribute location use a name.
        /// </summary>
        /// <param name="name">Attribute's name</param>
        /// <returns>Attribute's location</returns>
        public int GetAttributeLocation(string name)
        {
            return GL.GetAttribLocation(_program, name);
        }
        
        /// <summary>
        /// Return uniform location use a name.
        /// </summary>
        /// <param name="name">Uniform's name</param>
        /// <returns>Uniform's location</returns>
        public int GetUniformLocation(string name)
        {
            return GL.GetUniformLocation(_program, name);
        }
        
        /// <summary>
        /// Define a float value to a uniform.
        /// </summary>
        /// <param name="name">Name of uniform</param>
        /// <param name="value">New value of uniform</param>
        public void UseUniform1f(string name, float value)
        {
            GL.Uniform1(GetUniformLocation(name), value);
        }
        
        /// <summary>
        /// Define an int value to a uniform.
        /// </summary>
        /// <param name="name">Name of uniform</param>
        /// <param name="value">New value of uniform</param>
        public void UseUniform1(string name, int value)
        {
            GL.Uniform1(GetUniformLocation(name), value);
        }
        
        /// <summary>
        /// Define a vector 2D value to a uniform.
        /// </summary>
        /// <param name="name">Name of uniform</param>
        /// <param name="value">New value of uniform</param>
        public void UseUniformVec2(string name, Vector2 value)
        {
            GL.Uniform2(GetUniformLocation(name), value);
        }
        
        /// <summary>
        /// Define a vector 3D value to a uniform.
        /// </summary>
        /// <param name="name">Name of uniform</param>
        /// <param name="value">New value of uniform</param>
        public void UseUniformVec3(string name, Vector3 value)
        {
            GL.Uniform3(GetUniformLocation(name), value);
        }
        
        /// <summary>
        /// Define a matrix 3 value to a uniform use to define a transposition.
        /// </summary>
        /// <param name="name">Name of uniform</param>
        /// <param name="value">New value of uniform</param>
        public void UseUniformMat3(string name, Matrix3 value)
        {
            GL.UniformMatrix3(GetUniformLocation(name), true, ref value);
        }

        /// <summary>
        /// Define a matrix 4 value to a uniform use to define a projection.
        /// </summary>
        /// <param name="name">Name of uniform</param>
        /// <param name="value">New value of uniform</param>
        public void UseUniformProjectionMat4(string name, Matrix4 value)
        {
            GL.UniformMatrix4(GetUniformLocation(name), false, ref value);
        }

        /// <summary>
        /// Define a matrix 4 value to a uniform use to define a transposition.
        /// </summary>
        /// <param name="name">Name of uniform</param>
        /// <param name="value">New value of uniform</param>
        public void UseUniformMat4(string name, Matrix4 value)
        {
            GL.UniformMatrix4(GetUniformLocation(name), true, ref value);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="vertex"></param>
        /// <param name="fragment"></param>
        protected void Load(string vertex, string fragment)
        {
            _vertex = LoadShader(ShaderType.VertexShader, vertex);
            _fragment = LoadShader(ShaderType.FragmentShader, fragment);

            _program = GL.CreateProgram();
            Name = "shader" + _program;

            GL.AttachShader(_program, _vertex);
            GL.AttachShader(_program, _fragment);

            GL.LinkProgram(_program);

            GL.DeleteShader(_vertex);
            GL.DeleteShader(_fragment);
            
            Debug.Log("[INFO] " + Name + " initialized!");
        }

        /// <summary>
        /// OpenGL Loading of a shader.
        /// Type of shade is defined using <see cref="ShaderType"/>.
        /// </summary>
        /// <param name="type">type of loading shader</param>
        /// <param name="content">shader string content</param>
        /// <returns></returns>
        protected int LoadShader(ShaderType type, string content)
        {
            int shader = GL.CreateShader(type);
            GL.ShaderSource(shader, content);
            GL.CompileShader(shader);

            GL.GetShaderInfoLog(shader, out string shaderLogs);
            if (shaderLogs != "") Debug.LogError("[SHADER ERROR] " + shaderLogs);
            else Debug.Log("[SHADER] Successfully compile shader!");

            return shader;
        }

        /// <summary>
        /// Parse shader file in order to separate vertex part and fragment part use keywords
        /// 'vertex:' and 'fragment:'.
        /// Read file using path.
        /// </summary>
        /// <param name="path">Path to shader file</param>
        /// <returns>Vertex and fragment shader content</returns>
        protected (string, string) ParseShader(string path)
        {
            string[] lines = Resources.GetAllLines(path);
            string[] content = new string[2];
            int type = -1;
            
            foreach(string line in lines)
            {
                if (line.Contains(":") && !line.StartsWith("//"))
                {
                    // Define shader type by checking if line contain attribute

                    // Define type for vertex shader
                    if (line.Contains("vertex:")) type = (int)ShaderType.VertexShader;

                    // Define type for fragment shader
                    else if (line.Contains("fragment:")) type = (int)ShaderType.FragmentShader;
                }
                else
                {
                    // Add line to vertex content at array position
                    // (content[0] -> vertex content)
                    if (type == (int)ShaderType.VertexShader)
                    {
                        content[0] += line + "\n";
                    }

                    // Add line to fragment content at array position
                    // (content[1] -> fragment content)
                    else if (type == (int)ShaderType.FragmentShader)
                    {
                        content[1] += line + "\n";
                    }
                }
            }

            // Retun a struct that contains all parsed content
            return (content[0], content[1]);
        }

    }
}


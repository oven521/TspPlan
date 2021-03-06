#region netDxf, Copyright(C) 2014 Daniel Carvajal, Licensed under LGPL.

//                        netDxf library
// Copyright (C) 2014 Daniel Carvajal (haplokuon@gmail.com)
// 
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
// FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
// COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
// IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
// CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. 

#endregion

using System;

namespace netDxf
{

    /// <summary>
    /// Represents the base class for all dxf objects.
    /// </summary>
    public abstract class DxfObject
    {

        #region protected fields

        protected readonly string codeName;
        protected string handle;
        protected DxfObject owner;

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <c>DxfObject</c> class.
        /// </summary>
        /// <param name="codeName">Object name.</param>
        protected DxfObject(string codeName)
        {
            this.codeName = codeName;
            this.handle = null;
            this.owner = null;
        }

        #endregion

        #region public properties

        /// <summary>
        /// Gets the dxf entity type string.
        /// </summary>
        public string CodeName
        {
            get { return this.codeName; }
        }

        /// <summary>
        /// Gets the handle asigned to the dxf object.
        /// </summary>
        /// <remarks>The handle is a unique hexadecimal number asigned automatically to every dxf object.</remarks>
        public string Handle
        {
            get { return this.handle; }
            internal set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                this.handle = value;
            }
        }

        /// <summary>
        /// Gets the owner of the actual dxf object.
        /// </summary>
        internal virtual DxfObject Owner
        {
            get { return this.owner; }
            set { this.owner = value; }
        }

        #endregion

        #region public methods

        /// <summary>
        /// Asigns a handle to the object based in a integer counter.
        /// </summary>
        /// <param name="entityNumber">Number to asign.</param>
        /// <returns>Next avaliable entity number.</returns>
        /// <remarks>
        /// Some objects might consume more than one, is, for example, the case of polylines that will asign
        /// automatically a handle to its vertexes. The entity number will be converted to an hexadecimal number.
        /// </remarks>
        internal virtual long AsignHandle(long entityNumber)
        {
            this.handle = entityNumber.ToString("X");
            return entityNumber + 1;
        }

        #endregion

        #region overrides

        /// <summary>
        /// Obtains a string that represents the dxf object.
        /// </summary>
        /// <returns>A string text.</returns>
        public override string ToString()
        {
            return codeName;
        }

        #endregion
    }
}
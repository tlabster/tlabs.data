﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

using Microsoft.Extensions.Logging;

using Tlabs.Data;
using Tlabs.Data.Entity;
using Tlabs.Data.Entity.Intern;
using Tlabs.Data.Serialize;
using Tlabs.Dynamic;

namespace Tlabs.Data.Processing {

  /// <summary>Document validation exception.</summary>
  public class DocumentValidationException : GeneralException {
    /// <summary>Rule for which validation failed.</summary>
    public DocumentSchema.ValidationRule Rule { get; }
    /// <summary>Default ctor</summary>
    public DocumentValidationException(DocumentSchema.ValidationRule rule, Exception e) : base(e.Message, e) {
      this.Rule= rule;
    }
  }

  /// <summary><see cref="DocumentSchema"/> processor interface.</summary>
  public interface IDocSchemaProcessor {

    /// <summary><see cref="DocumentSchema"/>.</summary>
    DocumentSchema Schema { get; }
    /// <summary>Schema type Id.</summary>
    string Sid { get; }
    /// <summary><see cref="BaseDocument{T}"/>'s Body type resulting from <see cref="DocumentSchema"/>.</summary>
    Type BodyType { get; }
    /// <summary>Empty body object of Type: <see cref="BodyType"/>.</summary>
    object EmptyBody { get; }

    ///<summary><see cref="DynamicAccessor"/> to the body type.</summary>
    DynamicAccessor BodyAccessor { get; }

    ///<summary>Return <paramref name="doc"/>'s Body as object (according to its <see cref="DocumentSchema"/>).</summary>
    object LoadBodyObject<T>(T doc) where T : BaseDocument<T>;

    ///<summary>Set <paramref name="doc"/>'s Body to <paramref name="bodyObj"/>.</summary>
    /// <remarks>
    /// By specifying a <paramref name="setupData"/> delegate the caller can provide a custom dictionary of data beeing imported into
    /// the CalcNgn model. (Defaults to a dictionary representing all public properties of the <paramref name="bodyObj"/>.)
    /// </remarks>
    object UpdateBodyObject<T>(T doc, object bodyObj, Func<object, IDictionary<string, object>> setupData= null, int bufSz = 10*1024) where T : BaseDocument<T>;

    ///<summary>Check <paramref name="doc"/> against the validation rules and applies the result to the document status properties.</summary>
    void ApplyValidation<T>(T doc/*, Insuree insuree, ISource src*/, out object body) where T : BaseDocument<T>;


    ///<summary>Check <paramref name="doc"/> against the validation rules.</summary>
    ///<returns>true if valid. If invalid (false) the offending rule is set in <paramref name="rule"/>.</returns>
    bool CheckValidation<T>(T doc, out DocumentSchema.ValidationRule rule) where T : BaseDocument<T>;

    ///<summary>Check <paramref name="body"/> object against the validation rules.</summary>
    ///<returns>true if valid. If invalid (false) the offending rule is set in <paramref name="rule"/>.</returns>
    bool CheckValidation(object body, out DocumentSchema.ValidationRule rule) ;
  }


}